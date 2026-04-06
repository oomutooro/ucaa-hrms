import { Link, Outlet, useLocation } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

const navItems = [
  { to: "/dashboard", label: "Dashboard" },
  { to: "/employees", label: "Employees" },
  { to: "/departments", label: "Departments" },
  { to: "/leave", label: "Leave" },
  { to: "/payroll", label: "Payroll" },
  { to: "/shifts", label: "Shifts" },
  { to: "/documents", label: "Documents" },
];

export default function Layout() {
  const location = useLocation();
  const { email, role, logout } = useAuth();

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <h1>UCAA HRMS</h1>
          <p>Civil Aviation Authority</p>
        </div>
        <nav>
          {navItems.map((item) => (
            <Link
              key={item.to}
              to={item.to}
              className={location.pathname.startsWith(item.to) ? "nav-link active" : "nav-link"}
            >
              {item.label}
            </Link>
          ))}
        </nav>
      </aside>

      <main className="main-content">
        <header className="topbar">
          <div>
            <h2>Human Resource Management</h2>
            <span>{role ?? ""}</span>
          </div>
          <div className="topbar-actions">
            <span>{email}</span>
            <button onClick={logout}>Logout</button>
          </div>
        </header>
        <section className="page-body">
          <Outlet />
        </section>
      </main>
    </div>
  );
}
