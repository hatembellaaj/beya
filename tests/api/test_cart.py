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
        "price": 8.50,
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


def test_cart_add_update_remove(api_session, api_base_url, admin_headers, auth_headers):
    category = _create_category(api_session, api_base_url, admin_headers)
    category_id = category.get("categoryId") or category.get("id")
    article = _create_article(api_session, api_base_url, admin_headers, category_id)
    article_id = article.get("articleId") or article.get("id")

    add_response = api_session.post(
        f"{api_base_url}/api/cart",
        json={"articleId": article_id, "quantity": 2},
        headers=auth_headers,
        timeout=10,
    )
    assert add_response.status_code in {200, 201}
    cart_items = add_response.json()
    assert cart_items

    cart_item_id = cart_items[0].get("cartItemId") or cart_items[0].get("id")
    update_response = api_session.put(
        f"{api_base_url}/api/cart/{cart_item_id}",
        json={"quantity": 3},
        headers=auth_headers,
        timeout=10,
    )
    assert update_response.status_code in {200, 204}

    summary_response = api_session.get(
        f"{api_base_url}/api/cart/summary",
        headers=auth_headers,
        timeout=10,
    )
    assert summary_response.status_code == 200

    delete_response = api_session.delete(
        f"{api_base_url}/api/cart/{cart_item_id}",
        headers=auth_headers,
        timeout=10,
    )
    assert delete_response.status_code in {200, 204}
