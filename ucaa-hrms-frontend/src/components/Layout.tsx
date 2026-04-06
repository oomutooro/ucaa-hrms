import { Link, Outlet, useLocation } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

const navItems = [
  {
    to: "/dashboard", label: "Dashboard",
    icon: <svg viewBox="0 0 24 24" fill="currentColor" width="18" height="18"><path d="M3 13h8V3H3v10zm0 8h8v-6H3v6zm10 0h8V11h-8v10zm0-18v6h8V3h-8z"/></svg>
  },
  {
    to: "/employees", label: "Employee Management",
    icon: <svg viewBox="0 0 24 24" fill="currentColor" width="18" height="18"><path d="M12 12c2.7 0 4.8-2.1 4.8-4.8S14.7 2.4 12 2.4 7.2 4.5 7.2 7.2 9.3 12 12 12zm0 2.4c-3.2 0-9.6 1.6-9.6 4.8v2.4h19.2v-2.4c0-3.2-6.4-4.8-9.6-4.8z"/></svg>
  },
  {
    to: "/departments", label: "Departments",
    icon: <svg viewBox="0 0 24 24" fill="currentColor" width="18" height="18"><path d="M12 3L1 9l11 6 9-4.91V17h2V9L12 3zM5 13.18v4L12 21l7-3.82v-4L12 17l-7-3.82z"/></svg>
  },
  {
    to: "/job-architecture", label: "Job Architecture",
    icon: <svg viewBox="0 0 24 24" fill="currentColor" width="18" height="18"><path d="M20 6h-2.18c.07-.44.18-.88.18-1.36C18 2.1 15.9 0 13.36 0c-1.46 0-2.73.68-3.6 1.72L9 3 7.24 1.72C6.37.68 5.1 0 3.64 0 1.1 0-.01 2.1-.01 4.64c0 .48.11.92.18 1.36H0v2h20V6zm-9.5-2.5c.55-.84 1.49-1.4 2.56-1.4 1.38 0 2.5 1.12 2.5 2.5 0 .48-.33.9-.5 1.4H11V4.64c0-.72-.22-1.38-.5-1.87zm-7 1.1C3.5 3.12 4.62 2 6 2c1.07 0 2.01.56 2.56 1.4-.28.49-.5 1.15-.5 1.87V6H4.5c-.17-.5-.5-.92-.5-1.4zM0 20c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V8H0v12zm8-7h8v2H8v-2zm0-4h8v2H8V9zM4 9h2v2H4V9zm0 4h2v2H4v-2z"/></svg>
  },
  {
    to: "/recruitment", label: "Recruitment",
    icon: <svg viewBox="0 0 24 24" fill="currentColor" width="18" height="18"><path d="M16 11c1.66 0 2.99-1.34 2.99-3S17.66 5 16 5c-1.66 0-3 1.34-3 3s1.34 3 3 3zm-8 0c1.66 0 2.99-1.34 2.99-3S9.66 5 8 5C6.34 5 5 6.34 5 8s1.34 3 3 3zm0 2c-2.33 0-7 1.17-7 3.5V19h14v-2.5c0-2.33-4.67-3.5-7-3.5zm8 0c-.29 0-.62.02-.97.05 1.16.84 1.97 1.97 1.97 3.45V19h6v-2.5c0-2.33-4.67-3.5-7-3.5z"/></svg>
  },
  {
    to: "/leave", label: "Leave Management",
    icon: <svg viewBox="0 0 24 24" fill="currentColor" width="18" height="18"><path d="M17 12h-5v5h5v-5zM16 1v2H8V1H6v2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2h-1V1h-2zm3 18H5V8h14v11z"/></svg>
  },
  {
    to: "/payroll", label: "Payroll Management",
    icon: <svg viewBox="0 0 24 24" fill="currentColor" width="18" height="18"><path d="M11.8 10.9c-2.27-.59-3-1.2-3-2.15 0-1.09 1.01-1.85 2.7-1.85 1.78 0 2.44.85 2.5 2.1h2.21c-.07-1.72-1.12-3.3-3.21-3.81V3h-3v2.16c-1.94.42-3.5 1.68-3.5 3.61 0 2.31 1.91 3.46 4.7 4.13 2.5.6 3 1.48 3 2.41 0 .69-.49 1.79-2.7 1.79-2.06 0-2.87-.92-2.98-2.1h-2.2c.12 2.19 1.76 3.42 3.68 3.83V21h3v-2.15c1.95-.37 3.5-1.5 3.5-3.55 0-2.84-2.43-3.81-4.7-4.4z"/></svg>
  },
  {
    to: "/shifts", label: "Shifts",
    icon: <svg viewBox="0 0 24 24" fill="currentColor" width="18" height="18"><path d="M11.99 2C6.47 2 2 6.48 2 12s4.47 10 9.99 10C17.52 22 22 17.52 22 12S17.52 2 11.99 2zM12 20c-4.42 0-8-3.58-8-8s3.58-8 8-8 8 3.58 8 8-3.58 8-8 8zm.5-13H11v6l5.25 3.15.75-1.23-4.5-2.67V7z"/></svg>
  },
  {
    to: "/documents", label: "Documents",
    icon: <svg viewBox="0 0 24 24" fill="currentColor" width="18" height="18"><path d="M14 2H6c-1.1 0-1.99.9-1.99 2L4 20c0 1.1.89 2 1.99 2H18c1.1 0 2-.9 2-2V8l-6-6zm2 16H8v-2h8v2zm0-4H8v-2h8v2zm-3-5V3.5L18.5 9H13z"/></svg>
  },
];

export default function Layout() {
  const location = useLocation();
  const { email, role, logout } = useAuth();
  const initials = email ? email.substring(0, 2).toUpperCase() : "AU";

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="sidebar-logo">
          <svg viewBox="0 0 32 32" fill="none" width="28" height="28">
            <rect width="32" height="32" rx="8" fill="#f5a623"/>
            <path d="M8 8h6v16H8zM18 8h6v7h-6zM18 19h6v5h-6z" fill="#1a1f36"/>
          </svg>
          <span>UCAA HRMS</span>
        </div>
        <div className="sidebar-user">
          <div className="sidebar-avatar">{initials}</div>
          <div className="sidebar-username">{email?.split("@")[0] ?? "Admin"}</div>
          <div className="sidebar-role">{role ?? "User"}</div>
        </div>
        <div className="sidebar-section-label">Features</div>
        <ul className="sidebar-nav">
          {navItems.map(item => (
            <li key={item.to}>
              <Link to={item.to} className={location.pathname.startsWith(item.to) ? "active" : ""}>
                {item.icon}
                <span>{item.label}</span>
              </Link>
            </li>
          ))}
        </ul>
        <button className="sidebar-logout" onClick={logout}>
          <svg viewBox="0 0 24 24" fill="currentColor" width="16" height="16">
            <path d="M17 7l-1.41 1.41L18.17 11H8v2h10.17l-2.58 2.58L17 17l5-5-5-5zM4 5h8V3H4c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h8v-2H4V5z"/>
          </svg>
          <span>Log Out</span>
        </button>
      </aside>

      <div className="main-area">
        <div className="topbar">
          <button className="topbar-filter">&#9776;</button>
          <div className="topbar-search">
            <svg viewBox="0 0 24 24" fill="none" stroke="#9ca3af" strokeWidth="2" width="16" height="16">
              <circle cx="11" cy="11" r="8"/><path d="m21 21-4.35-4.35"/>
            </svg>
            <input placeholder="Search..." />
          </div>
          <div className="topbar-actions">
            <button className="topbar-icon-btn" style={{ background: "#1e3a8a" }}>
              <svg viewBox="0 0 24 24" fill="white" width="16" height="16">
                <path d="M12 22c1.1 0 2-.9 2-2h-4c0 1.1.9 2 2 2zm6-6v-5c0-3.07-1.64-5.64-4.5-6.32V4c0-.83-.67-1.5-1.5-1.5s-1.5.67-1.5 1.5v.68C7.63 5.36 6 7.92 6 11v5l-2 2v1h16v-1l-2-2z"/>
              </svg>
              <span className="badge">13</span>
            </button>
            <button className="topbar-icon-btn" style={{ background: "#f5a623" }}>
              <svg viewBox="0 0 24 24" fill="white" width="16" height="16">
                <path d="M19.43 12.98c.04-.32.07-.64.07-.98s-.03-.66-.07-.98l2.11-1.65c.19-.15.24-.42.12-.64l-2-3.46c-.12-.22-.39-.3-.61-.22l-2.49 1c-.52-.4-1.08-.73-1.69-.98l-.38-2.65C14.46 2.18 14.25 2 14 2h-4c-.25 0-.46.18-.49.42l-.38 2.65c-.61.25-1.17.59-1.69.98l-2.49-1c-.23-.09-.49 0-.61.22l-2 3.46c-.13.22-.07.49.12.64l2.11 1.65c-.04.32-.07.65-.07.98s.03.66.07.98l-2.11 1.65c-.19.15-.24.42-.12.64l2 3.46c.12.22.39.3.61.22l2.49-1c.52.4 1.08.73 1.69.98l.38 2.65c.03.24.24.42.49.42h4c.25 0 .46-.18.49-.42l.38-2.65c.61-.25 1.17-.59 1.69-.98l2.49 1c.23.09.49 0 .61-.22l2-3.46c.12-.22.07-.49-.12-.64l-2.11-1.65zM12 15.5c-1.93 0-3.5-1.57-3.5-3.5s1.57-3.5 3.5-3.5 3.5 1.57 3.5 3.5-1.57 3.5-3.5 3.5z"/>
              </svg>
            </button>
            <button className="topbar-icon-btn" style={{ background: "#3a7d44" }}>
              <svg viewBox="0 0 24 24" fill="white" width="16" height="16">
                <path d="M20 4H4c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 4l-8 5-8-5V6l8 5 8-5v2z"/>
              </svg>
              <span className="badge">13</span>
            </button>
          </div>
        </div>
        <div className="page-content">
          <Outlet />
        </div>
      </div>
    </div>
  );
}
