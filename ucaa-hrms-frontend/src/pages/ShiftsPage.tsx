import { FormEvent, useEffect, useMemo, useState } from "react";
import apiClient from "../api/apiClient";
import { AttendanceRecord, AttendanceSummary, Employee, ShiftAssignment } from "../types/models";

const shiftLabel: Record<number, string> = { 1: "Day", 2: "Night", 3: "Off" };
const attendanceStatusLabel: Record<number, string> = { 1: "Present", 2: "Late", 3: "Absent" };

function formatTime(value?: string | null) {
  if (!value) {
    return "-";
  }

  return value.length >= 5 ? value.slice(0, 5) : value;
}

function MetricCard({ label, value, helper }: { label: string; value: number | string; helper: string }) {
  return (
    <div className="card" style={{ padding: 18 }}>
      <div style={{ color: "#666", fontSize: 12 }}>{label}</div>
      <div style={{ fontSize: 28, fontWeight: 700, marginTop: 6 }}>{value}</div>
      <div style={{ color: "#8b8b8b", fontSize: 12, marginTop: 4 }}>{helper}</div>
    </div>
  );
}

export default function ShiftsPage() {
  const [items, setItems] = useState<ShiftAssignment[]>([]);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [summary, setSummary] = useState<AttendanceSummary | null>(null);
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<"attendance" | "roster">("attendance");
  const [form, setForm] = useState({ employeeId: "", shiftDate: "", shiftType: 1 });
  const [rotationForm, setRotationForm] = useState({ startDate: "", days: 7, employeeIds: [] as string[] });
  const [clockInForm, setClockInForm] = useState({ employeeId: "", attendanceDate: new Date().toISOString().slice(0, 10), notes: "" });
  const [clockOutNotes, setClockOutNotes] = useState<Record<string, string>>({});

  const load = async () => {
    setIsLoading(true);
    try {
      const [shiftResponse, employeeResponse, summaryResponse] = await Promise.all([
        apiClient.get<ShiftAssignment[]>("/shifts"),
        apiClient.get<Employee[]>("/employees"),
        apiClient.get<AttendanceSummary>("/shifts/summary"),
      ]);

      setItems(shiftResponse.data);
      setEmployees(employeeResponse.data);
      setSummary(summaryResponse.data);
      setError("");
    } catch (requestError: any) {
      setError(requestError.response?.data?.message ?? "Failed to load attendance data.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const sortedAssignments = useMemo(
    () => [...items].sort((left, right) => `${left.shiftDate}-${left.employeeName ?? ""}`.localeCompare(`${right.shiftDate}-${right.employeeName ?? ""}`)),
    [items]
  );

  const onAssignShift = async (event: FormEvent) => {
    event.preventDefault();
    try {
      await apiClient.post("/shifts/assign", { ...form, shiftType: Number(form.shiftType), employeeId: form.employeeId || null });
      setForm({ employeeId: "", shiftDate: "", shiftType: 1 });
      await load();
    } catch (requestError: any) {
      setError(requestError.response?.data?.message ?? "Failed to assign shift.");
    }
  };

  const onGenerateRotation = async (event: FormEvent) => {
    event.preventDefault();
    try {
      await apiClient.post("/shifts/rotation", {
        startDate: rotationForm.startDate,
        days: Number(rotationForm.days),
        employeeIds: rotationForm.employeeIds,
      });
      await load();
    } catch (requestError: any) {
      setError(requestError.response?.data?.message ?? "Failed to generate rotation.");
    }
  };

  const onClockIn = async (event: FormEvent) => {
    event.preventDefault();
    try {
      await apiClient.post("/shifts/attendance/clock-in", {
        employeeId: clockInForm.employeeId,
        attendanceDate: clockInForm.attendanceDate,
        notes: clockInForm.notes || null,
      });
      setClockInForm({ employeeId: "", attendanceDate: new Date().toISOString().slice(0, 10), notes: "" });
      await load();
    } catch (requestError: any) {
      setError(requestError.response?.data?.message ?? "Failed to clock in employee.");
    }
  };

  const onClockOut = async (attendance: AttendanceRecord) => {
    try {
      await apiClient.post(`/shifts/attendance/${attendance.id}/clock-out`, {
        notes: clockOutNotes[attendance.id] || null,
      });
      setClockOutNotes((current) => ({ ...current, [attendance.id]: "" }));
      await load();
    } catch (requestError: any) {
      setError(requestError.response?.data?.message ?? "Failed to clock out employee.");
    }
  };

  return (
    <div style={{ padding: "24px 32px" }}>
      <div className="page-title-block">
        <h2>Attendance &amp; Time Tracking</h2>
        <p>Shift planning, clock-in control, and worked-hours visibility.</p>
      </div>

      {error && <div className="card" style={{ marginBottom: 16, borderLeft: "4px solid #dc3545", color: "#721c24" }}>{error}</div>}

      <div style={{ display: "grid", gridTemplateColumns: "repeat(5, minmax(0, 1fr))", gap: 16, marginBottom: 24 }}>
        <MetricCard label="Scheduled Today" value={summary?.scheduledToday ?? 0} helper="Employees rostered" />
        <MetricCard label="Checked In" value={summary?.checkedInToday ?? 0} helper="Attendance recorded" />
        <MetricCard label="Late Today" value={summary?.lateToday ?? 0} helper="After grace period" />
        <MetricCard label="Open Records" value={summary?.pendingClockOuts ?? 0} helper="Awaiting clock-out" />
        <MetricCard label="Hours Today" value={summary?.totalHoursToday ?? 0} helper="Closed attendance hours" />
      </div>

      <div style={{ display: "flex", gap: 0, marginBottom: 20, borderBottom: "2px solid #e9ecef" }}>
        {([
          { key: "attendance", label: "Attendance Capture" },
          { key: "roster", label: "Roster Planning" },
        ] as const).map((tab) => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key)}
            style={{
              background: "none",
              border: "none",
              borderBottom: activeTab === tab.key ? "2px solid #3b5bdb" : "2px solid transparent",
              marginBottom: -2,
              padding: "10px 18px",
              cursor: "pointer",
              color: activeTab === tab.key ? "#3b5bdb" : "#666",
              fontWeight: activeTab === tab.key ? 600 : 400,
            }}
          >
            {tab.label}
          </button>
        ))}
      </div>

      {activeTab === "attendance" ? (
        <div className="content-grid">
          <form className="card" onSubmit={onClockIn} style={{ display: "flex", flexDirection: "column", gap: 12 }}>
            <div className="card-header" style={{ marginBottom: 4 }}>
              <span className="card-title">Clock In Employee</span>
            </div>
            <div className="form-group">
              <label>Employee</label>
              <select className="form-control" value={clockInForm.employeeId} onChange={(e) => setClockInForm({ ...clockInForm, employeeId: e.target.value })} required>
                <option value="">Select employee</option>
                {employees.map((employee) => (
                  <option key={employee.id} value={employee.id}>{employee.fullName}</option>
                ))}
              </select>
            </div>
            <div className="form-group">
              <label>Attendance Date</label>
              <input className="form-control" type="date" value={clockInForm.attendanceDate} onChange={(e) => setClockInForm({ ...clockInForm, attendanceDate: e.target.value })} required />
            </div>
            <div className="form-group">
              <label>Notes</label>
              <textarea className="form-control" rows={3} value={clockInForm.notes} onChange={(e) => setClockInForm({ ...clockInForm, notes: e.target.value })} />
            </div>
            <button type="submit" className="btn btn-primary">Clock In</button>
          </form>

          <div className="card wide">
            <div className="card-header">
              <span className="card-title">Recent Attendance</span>
            </div>
            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th>Date</th>
                    <th>Employee</th>
                    <th>Shift</th>
                    <th>Status</th>
                    <th>In</th>
                    <th>Out</th>
                    <th>Hours</th>
                    <th>Action</th>
                  </tr>
                </thead>
                <tbody>
                  {!isLoading && (summary?.recentAttendance.length ?? 0) === 0 ? (
                    <tr><td colSpan={8} style={{ textAlign: "center", color: "var(--text-muted)", padding: 24 }}>No attendance records yet</td></tr>
                  ) : summary?.recentAttendance.map((item) => (
                    <tr key={item.id}>
                      <td>{item.attendanceDate}</td>
                      <td>{item.employeeName}</td>
                      <td>{item.shiftType ? shiftLabel[item.shiftType] : "Office"}</td>
                      <td>
                        <span style={{
                          background: item.status === 2 ? "#fee2e2" : "#dcfce7",
                          color: item.status === 2 ? "#b91c1c" : "#166534",
                          padding: "3px 10px", borderRadius: 50, fontSize: 12, fontWeight: 600,
                        }}>{attendanceStatusLabel[item.status] ?? item.status}</span>
                      </td>
                      <td>{formatTime(item.checkInTime)}</td>
                      <td>{formatTime(item.checkOutTime)}</td>
                      <td>{item.hoursWorked}</td>
                      <td>
                        {item.isOpen ? (
                          <div style={{ display: "flex", gap: 8, alignItems: "center" }}>
                            <input
                              className="form-control"
                              style={{ width: 140 }}
                              placeholder="Clock-out note"
                              value={clockOutNotes[item.id] ?? ""}
                              onChange={(e) => setClockOutNotes((current) => ({ ...current, [item.id]: e.target.value }))}
                            />
                            <button type="button" className="btn btn-sm btn-primary" onClick={() => onClockOut(item)}>Clock Out</button>
                          </div>
                        ) : "Closed"}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>

          <div className="card wide" style={{ gridColumn: "1 / -1" }}>
            <div className="card-header">
              <span className="card-title">Monthly Time Rollup</span>
            </div>
            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th>Employee</th>
                    <th>Present Days</th>
                    <th>Late Days</th>
                    <th>Total Hours</th>
                  </tr>
                </thead>
                <tbody>
                  {!isLoading && (summary?.monthlyRollup.length ?? 0) === 0 ? (
                    <tr><td colSpan={4} style={{ textAlign: "center", color: "var(--text-muted)", padding: 24 }}>No monthly attendance yet</td></tr>
                  ) : summary?.monthlyRollup.map((item) => (
                    <tr key={item.employeeId}>
                      <td>{item.employeeName}</td>
                      <td>{item.presentDays}</td>
                      <td>{item.lateDays}</td>
                      <td>{item.totalHoursWorked}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      ) : (
        <div className="content-grid">
          <form className="card" onSubmit={onAssignShift} style={{ display: "flex", flexDirection: "column", gap: 12 }}>
            <div className="card-header" style={{ marginBottom: 4 }}>
              <span className="card-title">Assign Shift</span>
            </div>
            <div className="form-group">
              <label>Employee</label>
              <select className="form-control" value={form.employeeId} onChange={(e) => setForm({ ...form, employeeId: e.target.value })}>
                <option value="">Unassigned</option>
                {employees.map((employee) => (
                  <option key={employee.id} value={employee.id}>{employee.fullName}</option>
                ))}
              </select>
            </div>
            <div className="form-group">
              <label>Shift Date</label>
              <input className="form-control" type="date" value={form.shiftDate} onChange={(e) => setForm({ ...form, shiftDate: e.target.value })} required />
            </div>
            <div className="form-group">
              <label>Shift Type</label>
              <select className="form-control" value={form.shiftType} onChange={(e) => setForm({ ...form, shiftType: Number(e.target.value) })}>
                <option value={1}>Day</option>
                <option value={2}>Night</option>
                <option value={3}>Off</option>
              </select>
            </div>
            <button type="submit" className="btn btn-primary">Assign Shift</button>
          </form>

          <form className="card" onSubmit={onGenerateRotation} style={{ display: "flex", flexDirection: "column", gap: 12 }}>
            <div className="card-header" style={{ marginBottom: 4 }}>
              <span className="card-title">Generate Rotation</span>
            </div>
            <div className="form-group">
              <label>Start Date</label>
              <input className="form-control" type="date" value={rotationForm.startDate} onChange={(e) => setRotationForm({ ...rotationForm, startDate: e.target.value })} required />
            </div>
            <div className="form-group">
              <label>Days</label>
              <input className="form-control" type="number" min={1} value={rotationForm.days} onChange={(e) => setRotationForm({ ...rotationForm, days: Number(e.target.value) })} required />
            </div>
            <div className="form-group">
              <label>Employees</label>
              <select
                className="form-control"
                multiple
                value={rotationForm.employeeIds}
                onChange={(e) => setRotationForm({
                  ...rotationForm,
                  employeeIds: Array.from(e.target.selectedOptions).map((option) => option.value),
                })}
                style={{ minHeight: 140 }}
              >
                {employees.map((employee) => (
                  <option key={employee.id} value={employee.id}>{employee.fullName}</option>
                ))}
              </select>
            </div>
            <button type="submit" className="btn btn-primary">Generate Rotation</button>
          </form>

          <div className="card wide" style={{ gridColumn: "1 / -1" }}>
            <div className="card-header">
              <span className="card-title">Shift Assignments</span>
            </div>
            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th>Date</th>
                    <th>Shift Type</th>
                    <th>Employee</th>
                    <th>Code</th>
                  </tr>
                </thead>
                <tbody>
                  {!isLoading && sortedAssignments.length === 0 ? (
                    <tr><td colSpan={4} style={{ textAlign: "center", color: "var(--text-muted)", padding: 24 }}>No shift assignments</td></tr>
                  ) : sortedAssignments.map((item) => (
                    <tr key={item.id}>
                      <td>{item.shiftDate}</td>
                      <td>
                        <span style={{
                          background: item.shiftType === 1 ? "#dbeafe" : item.shiftType === 2 ? "#e0e7ff" : "#f3f4f6",
                          color: item.shiftType === 1 ? "var(--blue)" : item.shiftType === 2 ? "#4338ca" : "var(--text-muted)",
                          padding: "3px 10px", borderRadius: 50, fontSize: 12, fontWeight: 600,
                        }}>{shiftLabel[item.shiftType] ?? item.shiftType}</span>
                      </td>
                      <td>{item.employeeName ?? "-"}</td>
                      <td style={{ fontFamily: "monospace", fontSize: 12 }}>{item.shiftCode}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>

          <div className="card wide" style={{ gridColumn: "1 / -1" }}>
            <div className="card-header">
              <span className="card-title">Upcoming Assignments</span>
            </div>
            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th>Date</th>
                    <th>Shift</th>
                    <th>Employee</th>
                    <th>Code</th>
                  </tr>
                </thead>
                <tbody>
                  {!isLoading && (summary?.upcomingAssignments.length ?? 0) === 0 ? (
                    <tr><td colSpan={4} style={{ textAlign: "center", color: "var(--text-muted)", padding: 24 }}>No upcoming assignments</td></tr>
                  ) : summary?.upcomingAssignments.map((item) => (
                    <tr key={item.id}>
                      <td>{item.shiftDate}</td>
                      <td>{shiftLabel[item.shiftType] ?? item.shiftType}</td>
                      <td>{item.employeeName ?? "-"}</td>
                      <td style={{ fontFamily: "monospace", fontSize: 12 }}>{item.shiftCode}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
