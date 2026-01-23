import os
from urllib.parse import urlparse, urlunparse

import pytest
import requests
import urllib3


def _normalize_localhost_url(url: str) -> str:
    parsed = urlparse(url)
    if parsed.scheme == "https" and parsed.hostname in {"localhost", "127.0.0.1"}:
        return urlunparse(parsed._replace(scheme="http"))
    return url


def _base_url() -> str:
    url = os.environ.get("BASE_URL", "http://localhost:5000").rstrip("/")
    return _normalize_localhost_url(url)


def _ssl_verify_setting() -> bool | str:
    raw_value = os.environ.get("API_SSL_VERIFY", os.environ.get("SSL_VERIFY", "true"))
    value = raw_value.strip().lower()
    if value in {"0", "false", "no"}:
        return False
    if value in {"1", "true", "yes"}:
        return True
    return raw_value


@pytest.fixture(scope="session")
def api_base_url() -> str:
    return _base_url()


@pytest.fixture(scope="session")
def api_session() -> requests.Session:
    session = requests.Session()
    verify = _ssl_verify_setting()
    session.verify = verify
    if verify is False:
        urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)
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
