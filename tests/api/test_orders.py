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
    payload = {
        "name": f"Article {uuid.uuid4().hex[:6]}",
        "description": "QA",
        "price": 15.00,
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


def _add_to_cart(api_session, api_base_url, auth_headers, article_id):
    response = api_session.post(
        f"{api_base_url}/api/cart",
        json={"articleId": article_id, "quantity": 1},
        headers=auth_headers,
        timeout=10,
    )
    response.raise_for_status()


def test_create_order_and_history(api_session, api_base_url, admin_headers, auth_headers):
    category = _create_category(api_session, api_base_url, admin_headers)
    category_id = category.get("categoryId") or category.get("id")
    article = _create_article(api_session, api_base_url, admin_headers, category_id)
    article_id = article.get("articleId") or article.get("id")

    _add_to_cart(api_session, api_base_url, auth_headers, article_id)

    create_response = api_session.post(
        f"{api_base_url}/api/order",
        headers=auth_headers,
        timeout=10,
    )
    assert create_response.status_code in {200, 201}

    history_response = api_session.get(
        f"{api_base_url}/api/order",
        headers=auth_headers,
        timeout=10,
    )
    assert history_response.status_code == 200
