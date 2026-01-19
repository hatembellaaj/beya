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


def test_create_update_delete_category(api_session, api_base_url, admin_headers):
    category = _create_category(api_session, api_base_url, admin_headers)
    category_id = category.get("categoryId") or category.get("id")
    assert category_id is not None

    update_payload = {
        "categoryId": category_id,
        "name": f"Updated {category_id}",
        "description": "Updated",
    }
    update_response = api_session.put(
        f"{api_base_url}/api/categories/{category_id}",
        json=update_payload,
        headers=admin_headers,
        timeout=10,
    )
    assert update_response.status_code in {200, 204}

    delete_response = api_session.delete(
        f"{api_base_url}/api/categories/{category_id}",
        headers=admin_headers,
        timeout=10,
    )
    assert delete_response.status_code in {200, 204}
