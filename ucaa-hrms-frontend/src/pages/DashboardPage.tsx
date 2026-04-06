import { useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import { DashboardMetrics } from "../types/models";

export default function DashboardPage() {
  const [metrics, setMetrics] = useState<DashboardMetrics | null>(null);

  useEffect(() => {
    apiClient.get<DashboardMetrics>("/dashboard/metrics").then((res) => setMetrics(res.data)).catch(() => {});
  }, []);

  const m = metrics;

  return (
    <div>
      <div className="page-title-block">
        <h2>Dashboard</h2>
        <p>Human Resource Management Overview</p>
      </div>

      {/* Stat Cards */}
      <div className="stat-cards">
        <div className="stat-card yellow">
          <div className="stat-card-icon">
            <svg viewBox="0 0 24 24" fill="currentColor"><path d="M20 4H4c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 4l-8 5-8-5V6l8 5 8-5v2z"/></svg>
          </div>
          <div>
            <div className="stat-card-num">8</div>
            <div className="stat-card-label">Messages</div>
          </div>
        </div>
        <div className="stat-card blue">
          <div className="stat-card-icon">
            <svg viewBox="0 0 24 24" fill="currentColor"><path d="M20 6h-2.18c.07-.44.18-.88.18-1.34C18 2.54 15.96.5 13.5.5c-1.27 0-2.4.51-3.27 1.33L9 3.04 7.77 1.83C6.9 1.01 5.77.5 4.5.5 2.04.5 0 2.54 0 5c0 .45.11.9.18 1.34H0v13c0 1.1.9 2 2 2h20c1.1 0 2-.9 2-2V8c0-1.1-.9-2-2-2z"/></svg>
          </div>
          <div>
            <div className="stat-card-num">12</div>
            <div className="stat-card-label">Open Jobs</div>
          </div>
        </div>
        <div className="stat-card green">
          <div className="stat-card-icon">
            <svg viewBox="0 0 24 24" fill="currentColor"><path d="M12 12c2.7 0 4.8-2.1 4.8-4.8S14.7 2.4 12 2.4 7.2 4.5 7.2 7.2 9.3 12 12 12zm0 2.4c-3.2 0-9.6 1.6-9.6 4.8v2.4h19.2v-2.4c0-3.2-6.4-4.8-9.6-4.8z"/></svg>
          </div>
          <div>
            <div className="stat-card-num">34</div>
            <div className="stat-card-label">Candidates</div>
          </div>
        </div>
        <div className="stat-card dark">
          <div className="stat-card-icon">
            <svg viewBox="0 0 24 24" fill="currentColor"><path d="M14 2H6c-1.1 0-1.99.9-1.99 2L4 20c0 1.1.89 2 1.99 2H18c1.1 0 2-.9 2-2V8l-6-6zm2 16H8v-2h8v2zm0-4H8v-2h8v2zm-3-5V3.5L18.5 9H13z"/></svg>
          </div>
          <div>
            <div className="stat-card-num">56</div>
            <div className="stat-card-label">Resumes</div>
          </div>
        </div>
        <div className="stat-card navy">
          <div className="stat-card-icon">
            <svg viewBox="0 0 24 24" fill="currentColor"><path d="M12 12c2.7 0 4.8-2.1 4.8-4.8S14.7 2.4 12 2.4 7.2 4.5 7.2 7.2 9.3 12 12 12zm0 2.4c-3.2 0-9.6 1.6-9.6 4.8v2.4h19.2v-2.4c0-3.2-6.4-4.8-9.6-4.8z"/></svg>
          </div>
          <div>
            <div className="stat-card-num">{m?.totalEmployees ?? 0}</div>
            <div className="stat-card-label">Employees</div>
          </div>
        </div>
        <div className="stat-card blue">
          <div className="stat-card-icon">
            <svg viewBox="0 0 24 24" fill="currentColor"><path d="M17 12h-5v5h5v-5zM16 1v2H8V1H6v2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2h-1V1h-2zm3 18H5V8h14v11z"/></svg>
          </div>
          <div>
            <div className="stat-card-num">{m?.employeesOnLeave ?? 0}</div>
            <div className="stat-card-label">On Leave</div>
          </div>
        </div>
        <div className="stat-card green">
          <div className="stat-card-icon">
            <svg viewBox="0 0 24 24" fill="currentColor"><path d="M11.8 10.9c-2.27-.59-3-1.2-3-2.15 0-1.09 1.01-1.85 2.7-1.85 1.78 0 2.44.85 2.5 2.1h2.21c-.07-1.72-1.12-3.3-3.21-3.81V3h-3v2.16c-1.94.42-3.5 1.68-3.5 3.61 0 2.31 1.91 3.46 4.7 4.13 2.5.6 3 1.48 3 2.41 0 .69-.49 1.79-2.7 1.79-2.06 0-2.87-.92-2.98-2.1h-2.2c.12 2.19 1.76 3.42 3.68 3.83V21h3v-2.15c1.95-.37 3.5-1.5 3.5-3.55 0-2.84-2.43-3.81-4.7-4.4z"/></svg>
          </div>
          <div>
            <div className="stat-card-num">{m?.upcomingShifts ?? 0}</div>
            <div className="stat-card-label">Payrolls</div>
          </div>
        </div>
      </div>

      {/* Content Grid */}
      <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 16 }}>
        <div className="card">
          <div className="card-header">
            <span className="card-title">Employees</span>
            <button className="btn btn-sm btn-yellow">View All</button>
          </div>
          {["Alice Nakato", "Brian Otieno", "Carol Mwangi", "David Ssali"].map((name, i) => (
            <div className="emp-row" key={i}>
              <div className="emp-avatar">{name[0]}</div>
              <div className="emp-info">
                <div className="emp-name">{name}</div>
                <div className="emp-role">HR Officer</div>
              </div>
              <div className="emp-actions">
                <button className="btn btn-sm btn-primary">View</button>
                <button className="btn btn-sm" style={{ background: "#e5e7eb", color: "#374151" }}>Download</button>
              </div>
            </div>
          ))}
        </div>

        <div className="card">
          <div className="card-header">
            <span className="card-title">April Pay Runs</span>
            <button className="btn btn-sm btn-green">View All</button>
          </div>
          {[
            { name: "Alice Nakato", pct: 90, status: "paid" },
            { name: "Brian Otieno", pct: 60, status: "processing" },
            { name: "Carol Mwangi", pct: 100, status: "paid" },
            { name: "David Ssali", pct: 45, status: "processing" },
          ].map((p, i) => (
            <div key={i} style={{ marginBottom: 12 }}>
              <div style={{ display: "flex", justifyContent: "space-between", fontSize: 13 }}>
                <span>{p.name}</span>
                <span style={{ color: p.status === "paid" ? "var(--green)" : "var(--yellow)", fontWeight: 600 }}>
                  {p.status === "paid" ? "Paid" : "Processing"}
                </span>
              </div>
              <div className="progress-bar" style={{ marginTop: 4 }}>
                <div className="progress-bar-fill" style={{ width: `${p.pct}%`, background: p.status === "paid" ? "var(--green)" : "var(--yellow)" }} />
              </div>
            </div>
          ))}
        </div>

        <div className="card">
          <div className="card-header">
            <span className="card-title">Leave Requests</span>
          </div>
          <div className="table-wrap">
            <table>
              <thead><tr><th>Employee</th><th>Type</th><th>Status</th></tr></thead>
              <tbody>
                <tr><td>Alice Nakato</td><td>Annual</td><td style={{ color: "var(--yellow)", fontWeight: 600 }}>Pending</td></tr>
                <tr><td>Brian Otieno</td><td>Sick</td><td style={{ color: "var(--green)", fontWeight: 600 }}>Approved</td></tr>
                <tr><td>Carol Mwangi</td><td>Emergency</td><td style={{ color: "var(--red)", fontWeight: 600 }}>Declined</td></tr>
              </tbody>
            </table>
          </div>
        </div>

        <div className="card">
          <div className="card-header">
            <span className="card-title">Announcements</span>
          </div>
          {[
            { title: "Staff Retreat 2025", date: "May 12", desc: "All staff are required to attend the annual retreat." },
            { title: "Public Holiday", date: "Apr 22", desc: "Easter Monday - offices will be closed." },
            { title: "New HR Policy", date: "Apr 15", desc: "Updated leave policy effective from June 1st." },
          ].map((a, i) => (
            <div key={i} className="list-item" style={{ flexDirection: "column", alignItems: "flex-start", gap: 2 }}>
              <div style={{ display: "flex", justifyContent: "space-between", width: "100%" }}>
                <strong style={{ fontSize: 13 }}>{a.title}</strong>
                <span style={{ fontSize: 11, color: "var(--text-muted)" }}>{a.date}</span>
              </div>
              <span style={{ fontSize: 12, color: "var(--text-muted)" }}>{a.desc}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
