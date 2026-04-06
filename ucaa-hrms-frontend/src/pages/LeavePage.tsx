import { FormEvent, useEffect, useMemo, useState } from "react";
import apiClient from "../api/apiClient";
import { useAuth } from "../auth/AuthContext";
import { Employee, LeaveRequest, LeaveSummary } from "../types/models";

const leaveTypeLabel: Record<number, string> = {
  1: "Annual Leave",
  2: "Sick Leave",
  3: "Maternity Leave",
  4: "Paternity Leave",
  5: "Compassionate Leave",
  6: "Study Leave",
  7: "Emergency Leave",
};

const leaveStatusLabel: Record<number, string> = {
  1: "Pending",
  2: "Approved",
  3: "Rejected",
};

const leaveStatusColor: Record<number, { bg: string; fg: string }> = {
  1: { bg: "#fff3cd", fg: "#856404" },
  2: { bg: "#d4edda", fg: "#155724" },
  3: { bg: "#f8d7da", fg: "#721c24" },
};

const reviewerRoles = new Set(["Admin", "HR Manager", "Supervisor"]);

function daysBetween(startDate: string, endDate: string) {
  if (!startDate || !endDate) {
    return 0;
  }

  const start = new Date(startDate);
  const end = new Date(endDate);
  const milliseconds = end.getTime() - start.getTime();
  return milliseconds >= 0 ? Math.floor(milliseconds / 86400000) + 1 : 0;
}

function SummaryCard({ title, value, subtitle }: { title: string; value: number; subtitle: string }) {
  return (
    <div className="card" style={{ padding: 20 }}>
      <div style={{ color: "#666", fontSize: 13, marginBottom: 8 }}>{title}</div>
      <div style={{ fontSize: 28, fontWeight: 700, color: "#1a1f36" }}>{value}</div>
      <div style={{ fontSize: 12, color: "#8b8b8b", marginTop: 6 }}>{subtitle}</div>
    </div>
  );
}

function PolicyCard({ label, maxDaysPerRequest, onApply }: { label: string; maxDaysPerRequest: number; onApply: () => void }) {
  return (
    <div
      className="card"
      style={{
        padding: 18,
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        gap: 12,
        background: "linear-gradient(180deg, #ffffff 0%, #f8fbff 100%)",
      }}
    >
      <div>
        <div style={{ fontWeight: 600, color: "#1a1f36", marginBottom: 4 }}>{label}</div>
        <div style={{ fontSize: 12, color: "#666" }}>Maximum {maxDaysPerRequest} days per request</div>
      </div>
      <button className="btn btn-sm btn-primary" onClick={onApply}>Apply</button>
    </div>
  );
}

export default function LeavePage() {
  const { role } = useAuth();
  const canReview = reviewerRoles.has(role ?? "");

  const [items, setItems] = useState<LeaveRequest[]>([]);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [summary, setSummary] = useState<LeaveSummary | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [reviewingId, setReviewingId] = useState<string | null>(null);
  const [error, setError] = useState("");
  const [filters, setFilters] = useState({ search: "", leaveType: "all", status: "all" });
  const [form, setForm] = useState({
    employeeId: "",
    leaveType: 1,
    startDate: "",
    endDate: "",
    reason: "",
  });

  const load = async () => {
    setIsLoading(true);
    try {
      const [leaveResponse, employeeResponse, summaryResponse] = await Promise.all([
        apiClient.get<LeaveRequest[]>("/leave"),
        apiClient.get<Employee[]>("/employees"),
        apiClient.get<LeaveSummary>("/leave/summary"),
      ]);
      setItems(leaveResponse.data);
      setEmployees(employeeResponse.data);
      setSummary(summaryResponse.data);
    } catch {
      setError("Failed to load leave data.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const requestedDays = useMemo(
    () => daysBetween(form.startDate, form.endDate),
    [form.startDate, form.endDate]
  );

  const selectedPolicy = summary?.policyRules.find((rule) => rule.leaveType === Number(form.leaveType));
  const selectedEmployee = employees.find((employee) => employee.id === form.employeeId);

  const filteredItems = useMemo(() => {
    return items.filter((item) => {
      const matchesSearch =
        !filters.search ||
        item.employeeName.toLowerCase().includes(filters.search.toLowerCase()) ||
        item.reason.toLowerCase().includes(filters.search.toLowerCase());
      const matchesLeaveType = filters.leaveType === "all" || item.leaveType === Number(filters.leaveType);
      const matchesStatus = filters.status === "all" || item.status === Number(filters.status);
      return matchesSearch && matchesLeaveType && matchesStatus;
    });
  }, [filters, items]);

  const pendingItems = filteredItems.filter((item) => item.status === 1);

  const onSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setSaving(true);
    setError("");

    try {
      await apiClient.post("/leave/apply", { ...form, leaveType: Number(form.leaveType) });
      setForm({ employeeId: "", leaveType: 1, startDate: "", endDate: "", reason: "" });
      setShowForm(false);
      await load();
    } catch (requestError: any) {
      setError(requestError.response?.data?.message ?? "Failed to submit leave request.");
    } finally {
      setSaving(false);
    }
  };

  const reviewRequest = async (id: string, status: 2 | 3) => {
    const reviewerComment = window.prompt(
      status === 2 ? "Approval comment (optional):" : "Rejection reason:",
      ""
    );

    if (reviewerComment === null) {
      return;
    }

    setReviewingId(id);
    setError("");

    try {
      await apiClient.post(`/leave/${id}/review`, { status, reviewerComment });
      await load();
    } catch (requestError: any) {
      setError(requestError.response?.data?.message ?? "Failed to review leave request.");
    } finally {
      setReviewingId(null);
    }
  };

  return (
    <div style={{ padding: "24px 32px" }}>
      <div className="page-title-block">
        <h2>Leave Management</h2>
        <p>Policy visibility, approval workflow, and annual leave balance control.</p>
      </div>

      {error && (
        <div className="card" style={{ marginBottom: 16, borderLeft: "4px solid #dc3545", color: "#721c24" }}>
          {error}
        </div>
      )}

      <div style={{ display: "grid", gridTemplateColumns: "repeat(4, minmax(0, 1fr))", gap: 16, marginBottom: 24 }}>
        <SummaryCard title="Pending Requests" value={summary?.pendingRequests ?? 0} subtitle="Awaiting review" />
        <SummaryCard title="Approved Requests" value={summary?.approvedRequests ?? 0} subtitle="Approved to date" />
        <SummaryCard title="Employees On Leave" value={summary?.employeesCurrentlyOnLeave ?? 0} subtitle="Currently away today" />
        <SummaryCard title="Upcoming Approved" value={summary?.upcomingApprovedLeaves ?? 0} subtitle="Scheduled ahead" />
      </div>

      <div style={{ marginBottom: 24 }}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 12 }}>
          <h3 style={{ margin: 0, fontSize: 18 }}>Leave Policy</h3>
          <button className="btn btn-primary" onClick={() => setShowForm(true)}>+ New Request</button>
        </div>
        <div style={{ display: "grid", gridTemplateColumns: "repeat(3, minmax(0, 1fr))", gap: 16 }}>
          {(summary?.policyRules ?? []).map((rule) => (
            <PolicyCard
              key={rule.leaveType}
              label={rule.leaveTypeLabel}
              maxDaysPerRequest={rule.maxDaysPerRequest}
              onApply={() => {
                setForm((current) => ({ ...current, leaveType: rule.leaveType }));
                setShowForm(true);
              }}
            />
          ))}
        </div>
      </div>

      {showForm && (
        <div className="card" style={{ marginBottom: 24 }}>
          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", gap: 16 }}>
            <div>
              <h3 style={{ margin: 0, fontSize: 18 }}>Apply for Leave</h3>
              <p style={{ margin: "4px 0 0", color: "#666", fontSize: 13 }}>
                Submit a new request against the configured leave policy.
              </p>
            </div>
            <button className="btn btn-outline" onClick={() => setShowForm(false)}>Close</button>
          </div>

          <form onSubmit={onSubmit} style={{ marginTop: 16 }}>
            <div style={{ display: "grid", gridTemplateColumns: "repeat(2, minmax(0, 1fr))", gap: 16 }}>
              <div>
                <label className="form-label">Employee</label>
                <select
                  className="form-control"
                  value={form.employeeId}
                  onChange={(event) => setForm({ ...form, employeeId: event.target.value })}
                  required
                >
                  <option value="">Select employee</option>
                  {employees.map((employee) => (
                    <option key={employee.id} value={employee.id}>{employee.fullName}</option>
                  ))}
                </select>
              </div>
              <div>
                <label className="form-label">Leave Type</label>
                <select
                  className="form-control"
                  value={form.leaveType}
                  onChange={(event) => setForm({ ...form, leaveType: Number(event.target.value) })}
                >
                  {(summary?.policyRules ?? []).map((rule) => (
                    <option key={rule.leaveType} value={rule.leaveType}>{rule.leaveTypeLabel}</option>
                  ))}
                </select>
              </div>
              <div>
                <label className="form-label">Start Date</label>
                <input
                  className="form-control"
                  type="date"
                  value={form.startDate}
                  onChange={(event) => setForm({ ...form, startDate: event.target.value })}
                  required
                />
              </div>
              <div>
                <label className="form-label">End Date</label>
                <input
                  className="form-control"
                  type="date"
                  value={form.endDate}
                  onChange={(event) => setForm({ ...form, endDate: event.target.value })}
                  required
                />
              </div>
            </div>

            <div style={{ display: "grid", gridTemplateColumns: "repeat(3, minmax(0, 1fr))", gap: 16, marginTop: 16 }}>
              <div className="card" style={{ padding: 16, background: "#f8fbff" }}>
                <div style={{ fontSize: 12, color: "#666" }}>Requested days</div>
                <div style={{ fontSize: 24, fontWeight: 700 }}>{requestedDays}</div>
              </div>
              <div className="card" style={{ padding: 16, background: "#f8fbff" }}>
                <div style={{ fontSize: 12, color: "#666" }}>Policy maximum</div>
                <div style={{ fontSize: 24, fontWeight: 700 }}>{selectedPolicy?.maxDaysPerRequest ?? 0}</div>
              </div>
              <div className="card" style={{ padding: 16, background: "#f8fbff" }}>
                <div style={{ fontSize: 12, color: "#666" }}>Annual balance</div>
                <div style={{ fontSize: 24, fontWeight: 700 }}>
                  {selectedEmployee ? selectedEmployee.annualLeaveBalanceDays : "-"}
                </div>
              </div>
            </div>

            <div style={{ marginTop: 16 }}>
              <label className="form-label">Reason</label>
              <textarea
                className="form-control"
                value={form.reason}
                onChange={(event) => setForm({ ...form, reason: event.target.value })}
                rows={4}
                required
              />
            </div>

            <div style={{ display: "flex", gap: 12, marginTop: 16 }}>
              <button type="submit" className="btn btn-primary" disabled={saving}>
                {saving ? "Submitting..." : "Submit Request"}
              </button>
              <button type="button" className="btn btn-outline" onClick={() => setShowForm(false)}>
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      {canReview && pendingItems.length > 0 && (
        <div className="card" style={{ marginBottom: 24 }}>
          <div className="card-header">
            <span className="card-title">Approval Queue</span>
            <span style={{ fontSize: 12, color: "#666" }}>{pendingItems.length} pending request(s)</span>
          </div>
          <div style={{ display: "grid", gap: 12 }}>
            {pendingItems.slice(0, 5).map((item) => (
              <div key={item.id} style={{ border: "1px solid #e9ecef", borderRadius: 12, padding: 16, display: "flex", justifyContent: "space-between", gap: 16 }}>
                <div>
                  <div style={{ fontWeight: 600 }}>{item.employeeName}</div>
                  <div style={{ fontSize: 13, color: "#666", marginTop: 4 }}>
                    {leaveTypeLabel[item.leaveType]} | {item.startDate} to {item.endDate} | {item.requestedDays} day(s)
                  </div>
                  <div style={{ fontSize: 13, color: "#444", marginTop: 8 }}>{item.reason}</div>
                </div>
                <div style={{ display: "flex", gap: 8, alignItems: "center" }}>
                  <button
                    className="btn btn-sm"
                    style={{ background: "#198754", color: "#fff", borderColor: "#198754" }}
                    disabled={reviewingId === item.id}
                    onClick={() => reviewRequest(item.id, 2)}
                  >
                    Approve
                  </button>
                  <button
                    className="btn btn-sm"
                    style={{ background: "#dc3545", color: "#fff", borderColor: "#dc3545" }}
                    disabled={reviewingId === item.id}
                    onClick={() => reviewRequest(item.id, 3)}
                  >
                    Reject
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      <div className="card" style={{ marginBottom: 24 }}>
        <div className="card-header">
          <span className="card-title">Leave Requests</span>
          <div style={{ display: "flex", gap: 8 }}>
            <input
              className="form-control"
              style={{ minWidth: 220 }}
              placeholder="Search employee or reason"
              value={filters.search}
              onChange={(event) => setFilters({ ...filters, search: event.target.value })}
            />
            <select
              className="form-control"
              value={filters.leaveType}
              onChange={(event) => setFilters({ ...filters, leaveType: event.target.value })}
            >
              <option value="all">All types</option>
              {Object.entries(leaveTypeLabel).map(([value, label]) => (
                <option key={value} value={value}>{label}</option>
              ))}
            </select>
            <select
              className="form-control"
              value={filters.status}
              onChange={(event) => setFilters({ ...filters, status: event.target.value })}
            >
              <option value="all">All statuses</option>
              {Object.entries(leaveStatusLabel).map(([value, label]) => (
                <option key={value} value={value}>{label}</option>
              ))}
            </select>
          </div>
        </div>

        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Employee</th>
                <th>Type</th>
                <th>Dates</th>
                <th>Days</th>
                <th>Status</th>
                <th>Sick Pay</th>
                <th>Reason</th>
                <th>Review Note</th>
              </tr>
            </thead>
            <tbody>
              {!isLoading && filteredItems.length === 0 ? (
                <tr>
                  <td colSpan={8} style={{ textAlign: "center", color: "#8b8b8b", padding: 24 }}>
                    No leave requests match the current filters.
                  </td>
                </tr>
              ) : filteredItems.map((item) => (
                <tr key={item.id}>
                  <td>{item.employeeName}</td>
                  <td>{leaveTypeLabel[item.leaveType] ?? item.leaveType}</td>
                  <td>{item.startDate} to {item.endDate}</td>
                  <td>{item.requestedDays}</td>
                  <td>
                    <span
                      style={{
                        background: leaveStatusColor[item.status]?.bg ?? "#f1f3f5",
                        color: leaveStatusColor[item.status]?.fg ?? "#495057",
                        padding: "4px 10px",
                        borderRadius: 50,
                        fontSize: 12,
                        fontWeight: 600,
                      }}
                    >
                      {leaveStatusLabel[item.status] ?? "Pending"}
                    </span>
                  </td>
                  <td>{item.sickLeavePayPercent ? `${item.sickLeavePayPercent}%` : "-"}</td>
                  <td style={{ maxWidth: 220 }}>{item.reason}</td>
                  <td style={{ maxWidth: 220 }}>{item.reviewerComment || "-"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <div className="card">
        <div className="card-header">
          <span className="card-title">Annual Leave Balances</span>
          <span style={{ fontSize: 12, color: "#666" }}>{summary?.leaveBalances.length ?? 0} employees tracked</span>
        </div>
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Employee</th>
                <th>Department</th>
                <th>Entitlement</th>
                <th>Used</th>
                <th>Balance</th>
              </tr>
            </thead>
            <tbody>
              {(summary?.leaveBalances ?? []).slice(0, 12).map((balance) => (
                <tr key={balance.employeeId}>
                  <td>{balance.employeeName}</td>
                  <td>{balance.departmentName || "-"}</td>
                  <td>{balance.annualLeaveEntitlementDays}</td>
                  <td>{balance.annualLeaveUsedDays}</td>
                  <td style={{ fontWeight: 600 }}>{balance.annualLeaveBalanceDays}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
