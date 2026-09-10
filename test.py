from flask import Flask, request, jsonify
from flask_cors import CORS
from werkzeug.security import generate_password_hash, check_password_hash
import sqlite3
import os

app = Flask(__name__)
CORS(app)

DB_NAME = os.path.join(os.path.abspath(os.path.dirname(__file__)), 'game.db')

def get_db():
    conn = sqlite3.connect(DB_NAME)
    conn.row_factory = sqlite3.Row
    return conn

def init_db():
    with get_db() as conn:
        conn.execute('''
            CREATE TABLE IF NOT EXISTS users (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                username TEXT UNIQUE NOT NULL,
                password_hash TEXT NOT NULL,
                coins INTEGER DEFAULT 0,
                deaths INTEGER DEFAULT 0
            )
        ''')

init_db()

@app.route('/register', methods=['POST'])
def register():
    data = request.json or {}
    username = data.get('username')
    password = data.get('password')

    if not username or not password:
        return jsonify({"status": "error", "message": "Missing fields"}), 400

    hashed_pw = generate_password_hash(password)

    try:
        with get_db() as conn:
            conn.execute(
                "INSERT INTO users (username, password_hash) VALUES (?, ?)",
                (username, hashed_pw)
            )
        return jsonify({"status": "success", "message": "Registered!"})
    except sqlite3.IntegrityError:
        return jsonify({"status": "error", "message": "User already exists"}), 400

@app.route('/login', methods=['POST'])
def login():
    data = request.json or {}
    username = data.get('username')
    password = data.get('password', '')

    with get_db() as conn:
        user = conn.execute("SELECT * FROM users WHERE username = ?", (username,)).fetchone()

    if user and check_password_hash(user['password_hash'], password):
        return jsonify({
            "status": "success",
            "coins": user['coins'],
            "deaths": user['deaths']
        })
    return jsonify({"status": "error", "message": "Invalid credentials"}), 401

@app.route('/update_stats', methods=['POST'])
def update_stats():
    data = request.json or {}
    username = data.get('username')
    coins_earned = int(data.get('coins_earned', 0))
    deaths_count = int(data.get('deaths_count', 0))

    with get_db() as conn:
        user = conn.execute("SELECT * FROM users WHERE username = ?", (username,)).fetchone()
        if not user:
            return jsonify({"status": "error", "message": "User not found"}), 404

        conn.execute('''
            UPDATE users 
            SET coins = coins + ?, deaths = deaths + ? 
            WHERE username = ?
        ''', (coins_earned, deaths_count, username))

        updated_user = conn.execute("SELECT coins, deaths FROM users WHERE username = ?", (username,)).fetchone()

    return jsonify({
        "status": "success",
        "total_coins": updated_user['coins'],
        "total_deaths": updated_user['deaths']
    })

@app.route('/leaderboard', methods=['GET'])
def leaderboard():
    with get_db() as conn:
        top_users = conn.execute(
            "SELECT username, coins, deaths FROM users ORDER BY coins DESC LIMIT 7"
        ).fetchall()

    board = [{"username": u['username'], "coins": u['coins'], "deaths": u['deaths']} for u in top_users]
    return jsonify({"status": "success", "leaderboard": board})

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=5000)