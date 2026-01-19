import os

BASE_URL = os.environ.get("UI_BASE_URL", "http://localhost:5002").rstrip("/")
USERNAME = os.environ.get("UI_USER", "qa.user")
PASSWORD = os.environ.get("UI_PASSWORD", "Passw0rd!")
