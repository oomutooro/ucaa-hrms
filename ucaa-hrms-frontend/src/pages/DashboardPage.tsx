import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import apiClient from "../api/apiClient";
import { DashboardMetrics, Employee, LeaveRequest, PayrollRecord } from "../types/models";

const leaveTypeLabel: Record<number, string> = {
  1: "Annual",
  2: "Sick",
  3: "Maternity",
  4: "Paternity",
  5: "Compassionate",
  6: "Study",
  7: "Emergency"
};
const leaveStatusLabel: Record<number, string> = { 1: "Pending", 2: "Approved", 3: "Rejected" };

export default function DashboardPage() {
  const [metrics, setMetrics] = useState<DashboardMetrics | null>(null);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [leaveRequests, setLeaveRequests] = useState<LeaveRequest[]>([]);
  const [payrollRecords, setPayrollRecords] = useState<PayrollRecord[]>([]);

  useEffect(() => {
    Promise.all([
      apiClient.get<DashboardMetrics>("/dashboard/metrics"),
      apiClient.get<Employee[]>("/employees"),
      apiClient.get<LeaveRequest[]>("/leave"),
      apiClient.get<PayrollRecord[]>("/payroll"),
    ])
      .then(([metricsRes, employeesRes, leaveRes, payrollRes]) => {
        setMetrics(metricsRes.data);
        setEmployees(employeesRes.data);
        setLeaveRequests(leaveRes.data);
        setPayrollRecords(payrollRes.data);
      })
      .catch(() => {});
  }, []);

  const pendingLeaveCount = useMemo(
    () => leaveRequests.filter((request) => request.status === 1).length,
    [leaveRequests]
  );

  const recentEmployees = employees.slice(0, 5);
  const recentLeaveRequests = leaveRequests.slice(0, 5);
  const recentPayroll = payrollRecords.slice(0, 5);

  return (
    <div>
      <div className="page-title-block">
        <h2>Dashboard</h2>
        <p>Human Resource Management Overview</p>
      </div>

      <div className="stat-cards">
        <div className="stat-card navy">
          <div className="stat-card-icon">
            <svg viewBox="0 0 24 24" fill="currentColor"><path d="M12 12c2.7 0 4.8-2.1 4.8-4.8S14.7 2.4 12 2.4 7.2 4.5 7.2 7.2 9.3 12 12 12zm0 2.4c-3.2 0-9.6 1.6-9.6 4.8v2.4h19.2v-2.4c0-3.2-6.4-4.8-9.6-4.8z"/></svg>
          </div>
          <div>
            <div className="stat-card-num">{metrics?.totalEmployees ?? 0}</div>
            <div className="stat-card-label">Employees</div>
          </div>
        </div>

        <div className="stat-card blue">
          <div className="stat-card-icon">
            <svg viewBox="0 0 24 24" fill="currentColor"><path d="M17 12h-5v5h5v-5zM16 1v2H8V1H6v2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2h-1V1h-2zm3 18H5V8h14v11z"/></svg>
          </div>
          <div>
            <div className="stat-card-num">{metrics?.employeesOnLeave ?? 0}</div>
            <div className="stat-card-label">On Leave</div>
          </div>
        </div>

        <div className="stat-card yellow">
          <div className="stat-card-icon">
            <svg viewBox="0 0 24 24" fill="currentColor"><path d="M19 3h-1V1h-2v2H8V1H6v2H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H5V9h14v10zm-7-8c-1.66 0-3 1.34-3 3h2a1 1 0 1 1 2 0c0 .55-.45 1-1 1-.55 0-1 .45-1 1v1h2v-.17c1.16-.41 2-1.51 2-2.83 0-1.66-1.34-3-3-3z"/></svg>
          </div>
          <div>
            <div className="stat-card-num">{pendingLeaveCount}</div>
            <div className="stat-card-label">Pending Leave</div>
          </div>
        </div>

        <div className="stat-card green">
          <div className="stat-card-icon">
            <svg viewBox="0 0 24 24" fill="currentColor"><path d="M11.8 10.9c-2.27-.59-3-1.2-3-2.15 0-1.09 1.01-1.85 2.7-1.85 1.78 0 2.44.85 2.5 2.1h2.21c-.07-1.72-1.12-3.3-3.21-3.81V3h-3v2.16c-1.94.42-3.5 1.68-3.5 3.61 0 2.31 1.91 3.46 4.7 4.13 2.5.6 3 1.48 3 2.41 0 .69-.49 1.79-2.7 1.79-2.06 0-2.87-.92-2.98-2.1h-2.2c.12 2.19 1.76 3.42 3.68 3.83V21h3v-2.15c1.95-.37 3.5-1.5 3.5-3.55 0-2.84-2.43-3.81-4.7-4.4z"/></svg>
          </div>
          <div>
            <div className="stat-card-num">{payrollRecords.length}</div>
            <div className="stat-card-label">Payroll Records</div>
          </div>
        </div>
      </div>

      <div className="dashboard-grid">
        <div className="card">
          <div className="card-header">
            <span className="card-title">Recent Employees</span>
            <Link className="btn btn-sm btn-yellow" to="/employees">View All</Link>
          </div>
          {recentEmployees.length === 0 ? (
            <div className="empty-state">No employees yet. Create your first employee to populate this section.</div>
          ) : recentEmployees.map((employee) => (
            <div className="emp-row" key={employee.id}>
              <div className="emp-avatar">{employee.fullName[0]}</div>
              <div className="emp-info">
                <div className="emp-name">{employee.fullName}</div>
                <div className="emp-role">{employee.jobTitle}</div>
              </div>
            </div>
          ))}
        </div>

        <div className="card">
          <div className="card-header">
            <span className="card-title">Recent Payroll</span>
            <Link className="btn btn-sm btn-green" to="/payroll">View All</Link>
          </div>
          {recentPayroll.length === 0 ? (
            <div className="empty-state">No payroll records yet.</div>
          ) : recentPayroll.map((record) => (
            <div key={record.id} className="payroll-row">
              <span>{record.employeeName}</span>
              <strong>UGX {record.netPay.toLocaleString()}</strong>
            </div>
          ))}
        </div>

        <div className="card card-span-2">
          <div className="card-header">
            <span className="card-title">Recent Leave Requests</span>
            <Link className="btn btn-sm btn-primary" to="/leave">Open Leave</Link>
          </div>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Employee</th>
                  <th>Type</th>
                  <th>Start</th>
                  <th>End</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                {recentLeaveRequests.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="empty-table-cell">No leave requests yet.</td>
                  </tr>
                ) : recentLeaveRequests.map((item) => (
                  <tr key={item.id}>
                    <td>{item.employeeName}</td>
                    <td>{leaveTypeLabel[item.leaveType] ?? item.leaveType}</td>
                    <td>{item.startDate}</td>
                    <td>{item.endDate}</td>
                    <td>{leaveStatusLabel[item.status] ?? "Pending"}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
}
