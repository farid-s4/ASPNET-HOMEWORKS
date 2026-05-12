import { useState } from 'react'
import './App.css'

type AuthMode = 'login' | 'register' | 'welcome'

type RegisterForm = {
  userName: string
  email: string
  password: string
  confirmPassword: string
}

type LoginForm = {
  email: string
  password: string
}

const API_BASE_URL = 'http://localhost:5000';

function App() {
  const [mode, setMode] = useState<AuthMode>('login')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const [userName, setUserName] = useState('')
  const [registerForm, setRegisterForm] = useState<RegisterForm>({
    userName: '',
    email: '',
    password: '',
    confirmPassword: '',
  })
  const [loginForm, setLoginForm] = useState<LoginForm>({
    email: '',
    password: '',
  })
  const handleRegister = async (event: React.SyntheticEvent<HTMLFormElement>) => {
    event.preventDefault()
    setLoading(true)
    setError('')
    try {
      const response = await fetch(`${API_BASE_URL}/api/Auth/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          UserName: registerForm.userName,
          Email: registerForm.email,
          Password: registerForm.password,
          ConfirmPassword: registerForm.confirmPassword,
        }),
      })

      if (!response.ok) {
        const text = await response.text()
        setError(text || 'Ошибка регистрации')
        return
      }

      setUserName(registerForm.userName)
      setMode('welcome')
    } catch {
      setError('Не удалось подключиться к серверу')
    } finally {
      setLoading(false)
    }
  }

  const handleLogin = async (event: React.SyntheticEvent<HTMLFormElement>) => {
    event.preventDefault()
    setLoading(true)
    setError('')
    try {
      const response = await fetch(`${API_BASE_URL}/api/Auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          Email: loginForm.email,
          Password: loginForm.password,
        }),
      })

      if (!response.ok) {
        const text = await response.text()
        setError(text || 'Ошибка входа')
        return
      }

      setUserName(loginForm.email)
      setMode('welcome')
    } catch {
      setError('Не удалось подключиться к серверу')
    } finally {
      setLoading(false)
    }
  }

  const handleLogout = () => {
    setMode('login')
    setError('')
    setLoginForm({ email: '', password: '' })
    setUserName('')
  }

  return (
    <main className="auth-page">
      <div className="card">
        {mode === 'welcome' ? (
          <>
            <h1>Добро пожаловать</h1>
            <p className="subtitle">{userName}</p>
            <button type="button" onClick={handleLogout}>
              Выйти
            </button>
          </>
        ) : (
          <>
            <h1>{mode === 'login' ? 'Вход' : 'Регистрация'}</h1>

            {mode === 'register' ? (
              <form onSubmit={handleRegister}>
                <input
                  type="text"
                  placeholder="Имя пользователя"
                  value={registerForm.userName}
                  onChange={(e) =>
                    setRegisterForm((prev) => ({ ...prev, userName: e.target.value }))
                  }
                  required
                />
                <input
                  type="email"
                  placeholder="Email"
                  value={registerForm.email}
                  onChange={(e) =>
                    setRegisterForm((prev) => ({ ...prev, email: e.target.value }))
                  }
                  required
                />
                <input
                  type="password"
                  placeholder="Пароль"
                  value={registerForm.password}
                  onChange={(e) =>
                    setRegisterForm((prev) => ({ ...prev, password: e.target.value }))
                  }
                  required
                />
                <input
                  type="password"
                  placeholder="Подтверждение пароля"
                  value={registerForm.confirmPassword}
                  onChange={(e) =>
                    setRegisterForm((prev) => ({ ...prev, confirmPassword: e.target.value }))
                  }
                  required
                />
                <button type="submit" disabled={loading}>
                  {loading ? 'Подождите...' : 'Зарегистрироваться'}
                </button>
              </form>
            ) : (
              <form onSubmit={handleLogin}>
                <input
                  type="email"
                  placeholder="Email"
                  value={loginForm.email}
                  onChange={(e) =>
                    setLoginForm((prev) => ({ ...prev, email: e.target.value }))
                  }
                  required
                />
                <input
                  type="password"
                  placeholder="Пароль"
                  value={loginForm.password}
                  onChange={(e) =>
                    setLoginForm((prev) => ({ ...prev, password: e.target.value }))
                  }
                  required
                />
                <button type="submit" disabled={loading}>
                  {loading ? 'Подождите...' : 'Войти'}
                </button>
              </form>
            )}

            {error && <p className="error">{error}</p>}

            <button
              type="button"
              className="switch-mode"
              onClick={() => {
                setError('')
                setMode(mode === 'login' ? 'register' : 'login')
              }}
            >
              {mode === 'login' ? 'Нет аккаунта? Зарегистрироваться' : 'Уже есть аккаунт? Войти'}
            </button>
          </>
        )}
      </div>
    </main>
  )
}

export default App
