import uuid


def _create_category(api_session, api_base_url, admin_headers):
    payload = {"name": f"Cat {uuid.uuid4().hex[:6]}", "description": "QA"}
    response = api_session.post(
        f"{api_base_url}/api/categories",
        json=payload,
        headers=admin_headers,
        timeout=10,
    )
    response.raise_for_status()
    return response.json()


def _create_article(api_session, api_base_url, admin_headers, category_id):
    name = f"Pizza {uuid.uuid4().hex[:4]}"
    payload = {
        "name": name,
        "description": "QA",
        "price": 11.00,
        "categoryId": category_id,
    }
    response = api_session.post(
        f"{api_base_url}/api/articles",
        json=payload,
        headers=admin_headers,
        timeout=10,
    )
    response.raise_for_status()
    return response.json()


def test_search_by_name(api_session, api_base_url, admin_headers):
    category = _create_category(api_session, api_base_url, admin_headers)
    category_id = category.get("categoryId") or category.get("id")
    article = _create_article(api_session, api_base_url, admin_headers, category_id)
    name = article.get("name")

    response = api_session.get(
        f"{api_base_url}/api/articles",
        params={"name": name},
        timeout=10,
    )
    assert response.status_code == 200
    assert any(item.get("name") == name for item in response.json())
