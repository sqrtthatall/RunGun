from flask import Flask, request, jsonify
from flask_sqlalchemy import SQLAlchemy
from flask_cors import CORS
from werkzeug.security import generate_password_hash, check_password_hash
import os

app = Flask(__name__)
CORS(app)

# Настройка БД
basedir = os.path.abspath(os.path.dirname(__file__))
app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///' + os.path.join(basedir, 'game.db')
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
db = SQLAlchemy(app)


class User(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    username = db.Column(db.String(80), unique=True, nullable=False)
    password_hash = db.Column(db.String(128), nullable=False)
    high_score = db.Column(db.Integer, default=0)
    best_time = db.Column(db.Float, default=9999.0)


with app.app_context():
    db.create_all()


@app.route('/register', methods=['POST'])
def register():
    data = request.json
    username = data.get('username')
    password = data.get('password')

    if User.query.filter_by(username=username).first():
        return jsonify({"status": "error", "message": "User already exists"}), 400

    hashed_pw = generate_password_hash(password)
    new_user = User(username=username, password_hash=hashed_pw)
    db.session.add(new_user)
    db.session.commit()
    return jsonify({"status": "success", "message": "Registered!"})


@app.route('/login', methods=['POST'])
def login():
    data = request.json
    user = User.query.filter_by(username=data.get('username')).first()
    
    if user and check_password_hash(user.password_hash, data.get('password')):
        return jsonify({
            "status": "success", 
            "high_score": user.high_score,
            "best_time": user.best_time
        })
    return jsonify({"status": "error", "message": "Invalid credentials"}), 401


@app.route('/update_stats', methods=['POST'])
def update_stats():
    data = request.json
    # В реальной игре тут нужна проверка токена сессии, но для старта сойдет
    user = User.query.filter_by(username=data.get('username')).first()
    if not user:
        return jsonify({"status": "error"}), 404


    if data.get('score', 0) > user.high_score:
        user.high_score = data['score']
    
    if data.get('time', 9999.0) < user.best_time:
        user.best_time = data['time']
        
    db.session.commit()
    return jsonify({"status": "success"})


@app.route('/leaderboard', methods=['GET'])
def leaderboard():
    top_users = User.query.order_by(User.high_score.desc()).limit(10).all()
    board = [{"username": u.username, "score": u.high_score} for u in top_users]
    return jsonify({"status": "success", "leaderboard": board})

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=5000)