import os
import time

import pytest
import requests
import urllib3
from requests.adapters import HTTPAdapter
from urllib3.util.retry import Retry


def _normalize_localhost_url(url: str) -> str:
    if "://" not in url:
        return f"http://{url}"
    return url


def _base_url() -> str:
    url = os.environ.get("BASE_URL", "http://localhost:5000").rstrip("/")
    return _normalize_localhost_url(url)


def _ssl_verify_setting(base_url: str | None = None) -> bool | str:
    raw_value = os.environ.get("API_SSL_VERIFY") or os.environ.get("SSL_VERIFY")
    is_localhost_https = bool(
        base_url
        and (
            base_url.startswith("https://localhost")
            or base_url.startswith("https://127.0.0.1")
        )
    )
    if raw_value is None and base_url:
        if is_localhost_https:
            return False
        return True
    if raw_value is None:
        return True
    value = raw_value.strip().lower()
    if value in {"0", "false", "no"}:
        return False
    if value in {"1", "true", "yes"}:
        return False if is_localhost_https else True
    return raw_value


@pytest.fixture(scope="session")
def api_base_url() -> str:
    return _base_url()


@pytest.fixture(scope="session")
def api_session() -> requests.Session:
    session = requests.Session()
    verify = _ssl_verify_setting(_base_url())
    session.verify = verify
    if verify is False:
        urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)
    retries = Retry(
        total=3,
        connect=3,
        read=3,
        backoff_factor=0.3,
        status_forcelist=(502, 503, 504),
        allowed_methods=("GET", "POST", "PUT", "PATCH", "DELETE"),
    )
    adapter = HTTPAdapter(max_retries=retries)
    session.mount("http://", adapter)
    session.mount("https://", adapter)
    yield session
    session.close()


@pytest.fixture(scope="session")
def admin_credentials() -> dict:
    return {
        "userName": os.environ.get("ADMIN_USER", "admin@monresto.com"),
        "password": os.environ.get("ADMIN_PASSWORD", "Passw0rd!"),
    }


@pytest.fixture(scope="session")
def user_credentials() -> dict:
    return {
        "userName": os.environ.get("TEST_USER", "qa.user"),
        "email": os.environ.get("TEST_EMAIL", "qa.user@example.com"),
        "password": os.environ.get("TEST_PASSWORD", "Passw0rd!"),
    }


def _login(api_session: requests.Session, base_url: str, credentials: dict) -> str:
    response = api_session.post(
        f"{base_url}/api/account/login",
        json=credentials,
        timeout=10,
    )
    response.raise_for_status()
    payload = response.json()
    return payload["token"]


def _wait_for_api(api_session: requests.Session, base_url: str, timeout_s: int = 30) -> None:
    deadline = time.monotonic() + timeout_s
    last_error: Exception | None = None
    while time.monotonic() < deadline:
        try:
            response = api_session.get(f"{base_url}/api/categories", timeout=5)
            if response.status_code < 500:
                return
        except requests.RequestException as exc:
            last_error = exc
        time.sleep(1)
    if last_error:
        raise RuntimeError(
            "API indisponible ou connexion fermée. Vérifiez que le backend est démarré "
            "et écoute sur BASE_URL."
        ) from last_error
    raise RuntimeError(
        "API indisponible ou connexion fermée. Vérifiez que le backend est démarré "
        "et écoute sur BASE_URL."
    )


@pytest.fixture(scope="session", autouse=True)
def ensure_api_ready(api_session: requests.Session, api_base_url: str) -> None:
    _wait_for_api(api_session, api_base_url)


@pytest.fixture(scope="session")
def auth_token(api_session: requests.Session, api_base_url: str, user_credentials: dict) -> str:
    api_session.post(
        f"{api_base_url}/api/account/register",
        json=user_credentials,
        timeout=10,
    )
    return _login(api_session, api_base_url, {
        "userName": user_credentials["userName"],
        "password": user_credentials["password"],
    })


@pytest.fixture(scope="session")
def admin_token(api_session: requests.Session, api_base_url: str, admin_credentials: dict) -> str:
    return _login(api_session, api_base_url, admin_credentials)


@pytest.fixture()

def auth_headers(auth_token: str) -> dict:
    return {"Authorization": f"Bearer {auth_token}"}


@pytest.fixture()

def admin_headers(admin_token: str) -> dict:
    return {"Authorization": f"Bearer {admin_token}"}
