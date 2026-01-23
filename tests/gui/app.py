from __future__ import annotations

import os
import subprocess
from datetime import datetime
from pathlib import Path

from flask import Flask, redirect, render_template, request, send_from_directory, url_for

APP_ROOT = Path(__file__).resolve().parent
REPORTS_DIR = APP_ROOT / "reports"

app = Flask(__name__)


@app.get("/")
def index():
    return render_template("index.html")


@app.post("/run/<suite>")
def run_suite(suite: str):
    base_url = request.form.get("base_url", "").strip()
    ui_base_url = request.form.get("ui_base_url", "").strip()
    ssl_verify = request.form.get("ssl_verify", "").strip().lower()
    timestamp = datetime.utcnow().strftime("%Y%m%d-%H%M%S")
    report_name = f"{suite}-{timestamp}.html"
    report_path = REPORTS_DIR / report_name

    if suite == "api":
        test_path = "tests/api"
        env_var = {"BASE_URL": base_url} if base_url else {}
        if ssl_verify in {"0", "false", "no"}:
            env_var["API_SSL_VERIFY"] = "false"
    elif suite == "ui":
        test_path = "tests/ui"
        env_var = {"UI_BASE_URL": ui_base_url} if ui_base_url else {}
    else:
        return redirect(url_for("index"))

    env = os.environ.copy()
    env.update(env_var)

    cmd = [
        "pytest",
        test_path,
        "--html",
        str(report_path),
        "--self-contained-html",
    ]

    REPORTS_DIR.mkdir(parents=True, exist_ok=True)
    subprocess.run(cmd, env=env, check=False)

    return redirect(url_for("show_report", report_name=report_name))


@app.get("/report/<report_name>")
def show_report(report_name: str):
    return render_template("result.html", report_name=report_name)


@app.get("/reports/<path:report_name>")
def download_report(report_name: str):
    return send_from_directory(REPORTS_DIR, report_name)


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5050, debug=True)
