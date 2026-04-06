import { FormEvent, useState } from "react";
import { Navigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export default function LoginPage() {
  const { login, isAuthenticated } = useAuth();
  const [email, setEmail] = useState("admin@ucaa.go.ug");
  const [password, setPassword] = useState("Admin@12345");
  const [remember, setRemember] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  if (isAuthenticated) return <Navigate to="/dashboard" replace />;

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      await login(email, password);
    } catch {
      setError("Invalid email or password. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-shell">
      <div className="login-left">
        <div className="login-left-overlay" />
        <div className="login-logo">
          <svg viewBox="0 0 32 32" fill="none" width="32" height="32">
            <rect width="32" height="32" rx="8" fill="#f5a623" />
            <path d="M8 8h6v16H8zM18 8h6v7h-6zM18 19h6v5h-6z" fill="#1a1f36" />
          </svg>
          <span>UCAA&nbsp;HRMS</span>
        </div>
        <div className="login-left-content">
          <h2 className="login-hero-title">HR Management Platform</h2>
          <p className="login-hero-sub">
            Manage all employees, payrolls, leave requests and other<br />
            human resource operations from one place.
          </p>
          <div className="login-slider-dots">
            <div className="login-dot active" />
            <div className="login-dot" />
            <div className="login-dot" />
          </div>
        </div>
      </div>
      <div className="login-right">
        <div className="login-form">
          <h1 className="login-title">Login</h1>
          <p className="login-sub">Login to your account.</p>
          <form onSubmit={onSubmit}>
            <div className="form-group">
              <label>E-mail Address</label>
              <input className="form-control" type="email" value={email}
                onChange={e => setEmail(e.target.value)} placeholder="your@email.com" required />
            </div>
            <div className="form-group">
              <label>Password</label>
              <input className="form-control" type="password" value={password}
                onChange={e => setPassword(e.target.value)} placeholder="••••••••" required />
            </div>
            <div className="login-remember">
              <label>
                <input type="checkbox" checked={remember} onChange={e => setRemember(e.target.checked)} />
                &nbsp;Remember me
              </label>
              <a href="#reset" className="login-link">Reset Password?</a>
            </div>
            {error && (
              <div style={{ background: "#fee2e2", color: "#dc2626", borderRadius: 8, padding: "10px 14px", marginBottom: 16, fontSize: 13 }}>
                {error}
              </div>
            )}
            <button type="submit" className="btn btn-primary"
              style={{ width: "100%", padding: "12px", fontSize: 15 }} disabled={loading}>
              {loading ? "Signing in…" : "Sign In"}
            </button>
          </form>
          <p style={{ textAlign: "center", marginTop: 20, fontSize: 13, color: "#6b7280" }}>
            Don't have an account yet?{" "}
            <a href="#register" className="login-link">Join UCAA HRMS today.</a>
          </p>
        </div>
      </div>
    </div>
  );
}
