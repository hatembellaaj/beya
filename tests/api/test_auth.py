import uuid


def test_register_and_login(api_session, api_base_url):
    unique = uuid.uuid4().hex[:8]
    payload = {
        "userName": f"qa_{unique}",
        "email": f"qa_{unique}@example.com",
        "password": "Passw0rd!",
    }
    register_response = api_session.post(
        f"{api_base_url}/api/account/register",
        json=payload,
        timeout=10,
    )
    assert register_response.status_code == 200

    login_response = api_session.post(
        f"{api_base_url}/api/account/login",
        json={"userName": payload["userName"], "password": payload["password"]},
        timeout=10,
    )
    assert login_response.status_code == 200
    assert "token" in login_response.json()


def test_login_invalid_password(api_session, api_base_url, user_credentials):
    api_session.post(
        f"{api_base_url}/api/account/register",
        json=user_credentials,
        timeout=10,
    )
    response = api_session.post(
        f"{api_base_url}/api/account/login",
        json={"userName": user_credentials["userName"], "password": "BadPass"},
        timeout=10,
    )
    assert response.status_code == 401
