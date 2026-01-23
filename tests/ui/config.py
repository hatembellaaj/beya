import os
from urllib.parse import urlparse, urlunparse

def _normalize_localhost_url(url: str) -> str:
    parsed = urlparse(url)
    if parsed.scheme == "https" and parsed.hostname in {"localhost", "127.0.0.1"}:
        return urlunparse(parsed._replace(scheme="http"))
    return url


BASE_URL = _normalize_localhost_url(
    os.environ.get("UI_BASE_URL", "http://localhost:5002").rstrip("/")
)
USERNAME = os.environ.get("UI_USER", "qa.user")
PASSWORD = os.environ.get("UI_PASSWORD", "Passw0rd!")
