import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import apiClient from "../api/apiClient";
import { useAuth } from "../auth/AuthContext";
import {
  AttendanceSummary,
  BenefitEnrollment,
  Department,
  Employee,
  EmployeeOnboarding,
  LeaveRequest,
  PayrollRecord,
  Position,
} from "../types/models";

const leaveStatusLabel: Record<number, string> = { 1: "Pending", 2: "Approved", 3: "Rejected" };
const onboardingStatusLabel: Record<number, string> = { 1: "Not Started", 2: "In Progress", 3: "Completed" };

const reviewerRoles = new Set(["Admin", "HR Manager", "Supervisor"]);
const managerRoles = new Set(["Admin", "HR Manager"]);

export default function EmployeeProfilePage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { role } = useAuth();

  const [employee, setEmployee] = useState<Employee | null>(null);
  const [departments, setDepartments] = useState<Department[]>([]);
  const [positions, setPositions] = useState<Position[]>([]);
  const [leaves, setLeaves] = useState<LeaveRequest[]>([]);
  const [onboardings, setOnboardings] = useState<EmployeeOnboarding[]>([]);
  const [attendanceSummary, setAttendanceSummary] = useState<AttendanceSummary | null>(null);
  const [payroll, setPayroll] = useState<PayrollRecord[]>([]);
  const [benefits, setBenefits] = useState<BenefitEnrollment[]>([]);

  const [activeTab, setActiveTab] = useState<"profile" | "leave" | "attendance" | "onboarding" | "payroll" | "benefits">("profile");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const [form, setForm] = useState({
    fullName: "",
    email: "",
    phoneNumber: "",
    dateOfBirth: "",
    firstEmploymentDate: "",
    jobLevel: 9,
    departmentId: "",
    positionId: "",
    jobTitle: "",
    employmentType: 1,
  });

  const canReviewLeave = reviewerRoles.has(role ?? "");
  const canManage = managerRoles.has(role ?? "");

  const load = useCallback(async () => {
    if (!id) {
      setError("Employee id missing.");
      setLoading(false);
      return;
    }

    setLoading(true);
    setError("");

    try {
      const employeeReq = apiClient.get<Employee>(`/employees/${id}`);
      const departmentsReq = apiClient.get<Department[]>("/departments");
      const positionsReq = apiClient.get<Position[]>("/jobarchitecture/positions");
      const leavesReq = apiClient.get<LeaveRequest[]>("/leave");
      const onboardingReq = apiClient.get<EmployeeOnboarding[]>("/onboarding/records");
      const attendanceReq = apiClient.get<AttendanceSummary>("/shifts/summary");
      const benefitsReq = apiClient.get<BenefitEnrollment[]>("/benefits/enrollments");

      const results = await Promise.allSettled([
        employeeReq,
        departmentsReq,
        positionsReq,
        leavesReq,
        onboardingReq,
        attendanceReq,
        benefitsReq,
      ]);

      const employeeResult = results[0];
      if (employeeResult.status !== "fulfilled") {
        throw new Error("Failed to load employee profile.");
      }

      const employeeData = employeeResult.value.data;
      setEmployee(employeeData);
      setForm({
        fullName: employeeData.fullName,
        email: employeeData.email,
        phoneNumber: employeeData.phoneNumber,
        dateOfBirth: employeeData.dateOfBirth,
        firstEmploymentDate: employeeData.firstEmploymentDate,
        jobLevel: employeeData.jobLevel,
        departmentId: employeeData.departmentId,
        positionId: employeeData.positionId ?? "",
        jobTitle: employeeData.jobTitle,
        employmentType: employeeData.employmentType,
      });

      if (results[1].status === "fulfilled") setDepartments(results[1].value.data);
      if (results[2].status === "fulfilled") setPositions(results[2].value.data);
      if (results[3].status === "fulfilled") setLeaves(results[3].value.data.filter((x) => x.employeeId === employeeData.id));
      if (results[4].status === "fulfilled") setOnboardings(results[4].value.data.filter((x) => x.employeeId === employeeData.id));
      if (results[5].status === "fulfilled") setAttendanceSummary(results[5].value.data);
      if (results[6].status === "fulfilled") setBenefits(results[6].value.data.filter((x) => x.employeeId === employeeData.id));

      try {
        const payrollRes = await apiClient.get<PayrollRecord[]>("/payroll");
        setPayroll(payrollRes.data.filter((x) => x.employeeId === employeeData.id));
      } catch {
        setPayroll([]);
      }
    } catch (loadError: any) {
      setError(loadError?.message ?? "Failed to load employee profile.");
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    load();
  }, [load]);

  const assignableDepartments = useMemo(
    () => departments.filter((department) => Boolean(department.parentDepartmentId)),
    [departments]
  );

  const positionsForDepartment = useMemo(
    () => positions.filter((position) => position.departmentId === form.departmentId),
    [positions, form.departmentId]
  );

  const attendanceForEmployee = useMemo(
    () => (attendanceSummary?.recentAttendance ?? []).filter((x) => x.employeeId === id),
    [attendanceSummary, id]
  );

  const attendanceRollup = useMemo(
    () => attendanceSummary?.monthlyRollup.find((x) => x.employeeId === id),
    [attendanceSummary, id]
  );

  const saveProfile = async (event: FormEvent) => {
    event.preventDefault();
    if (!id) return;

    setSaving(true);
    setError("");
    try {
      const response = await apiClient.put<Employee>(`/employees/${id}`, {
        fullName: form.fullName,
        email: form.email,
        phoneNumber: form.phoneNumber,
        dateOfBirth: form.dateOfBirth,
        firstEmploymentDate: form.firstEmploymentDate,
        jobLevel: Number(form.jobLevel),
        departmentId: form.departmentId,
        positionId: form.positionId || null,
        jobTitle: form.jobTitle,
        employmentType: Number(form.employmentType),
      });
      setEmployee(response.data);
    } catch (requestError: any) {
      setError(requestError.response?.data?.message ?? "Failed to update employee profile.");
    } finally {
      setSaving(false);
    }
  };

  const reviewLeave = async (leaveId: string, status: 2 | 3) => {
    const reviewerComment = window.prompt(status === 2 ? "Approval comment (optional)" : "Rejection reason", "");
    if (reviewerComment === null) return;

    try {
      await apiClient.post(`/leave/${leaveId}/review`, { status, reviewerComment });
      await load();
    } catch (requestError: any) {
      setError(requestError.response?.data?.message ?? "Failed to review leave request.");
    }
  };

  const updateOnboardingStatus = async (recordId: string, status: number) => {
    try {
      await apiClient.patch(`/onboarding/records/${recordId}/status`, { status });
      await load();
    } catch (requestError: any) {
      setError(requestError.response?.data?.message ?? "Failed to update onboarding status.");
    }
  };

  if (loading) {
    return <div style={{ padding: "24px 32px" }}>Loading employee profile...</div>;
  }

  if (!employee) {
    return <div style={{ padding: "24px 32px", color: "var(--red)" }}>Employee not found.</div>;
  }

  return (
    <div style={{ padding: "24px 32px" }}>
      <div className="page-title-block" style={{ marginBottom: 12 }}>
        <h2>{employee.fullName}</h2>
        <p>{employee.employeeId} | {employee.jobTitle} | {employee.departmentName}</p>
      </div>

      <div style={{ display: "flex", gap: 10, marginBottom: 16 }}>
        <button className="btn btn-outline" onClick={() => navigate("/employees")}>Back to Employees</button>
        <Link className="btn btn-outline" to="/leave">Open Leave Module</Link>
        <Link className="btn btn-outline" to="/onboarding">Open Onboarding Module</Link>
        <Link className="btn btn-outline" to="/shifts">Open Attendance Module</Link>
        <Link className="btn btn-outline" to="/payroll">Open Payroll Module</Link>
      </div>

      {error && (
        <div className="card" style={{ marginBottom: 16, color: "var(--red)", borderLeft: "4px solid var(--red)" }}>
          {error}
        </div>
      )}

      <div style={{ display: "flex", gap: 8, marginBottom: 16, flexWrap: "wrap" }}>
        <button className="btn" onClick={() => setActiveTab("profile")}>Profile</button>
        <button className="btn" onClick={() => setActiveTab("leave")}>Leave</button>
        <button className="btn" onClick={() => setActiveTab("attendance")}>Attendance</button>
        <button className="btn" onClick={() => setActiveTab("onboarding")}>Onboarding</button>
        <button className="btn" onClick={() => setActiveTab("payroll")}>Payroll</button>
        <button className="btn" onClick={() => setActiveTab("benefits")}>Benefits</button>
      </div>

      {activeTab === "profile" && (
        <form className="card" onSubmit={saveProfile} style={{ display: "grid", gap: 12 }}>
          <div className="card-header">
            <span className="card-title">Employee Profile</span>
            <span style={{ color: "var(--text-muted)", fontSize: 12 }}>
              Salary Grade: {employee.salaryGradeCode} ({employee.salaryGradeTitle})
            </span>
          </div>

          <div style={{ display: "grid", gridTemplateColumns: "repeat(2, minmax(0, 1fr))", gap: 12 }}>
            <div className="form-group"><label>Full Name</label><input className="form-control" value={form.fullName} onChange={(e) => setForm({ ...form, fullName: e.target.value })} required /></div>
            <div className="form-group"><label>Email</label><input className="form-control" type="email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} required /></div>
            <div className="form-group"><label>Phone Number</label><input className="form-control" value={form.phoneNumber} onChange={(e) => setForm({ ...form, phoneNumber: e.target.value })} required /></div>
            <div className="form-group"><label>Job Title</label><input className="form-control" value={form.jobTitle} onChange={(e) => setForm({ ...form, jobTitle: e.target.value })} required /></div>
            <div className="form-group"><label>Date of Birth</label><input className="form-control" type="date" value={form.dateOfBirth} onChange={(e) => setForm({ ...form, dateOfBirth: e.target.value })} required /></div>
            <div className="form-group"><label>First Employment Date</label><input className="form-control" type="date" value={form.firstEmploymentDate} onChange={(e) => setForm({ ...form, firstEmploymentDate: e.target.value })} required /></div>
            <div className="form-group"><label>Job Level</label><input className="form-control" type="number" min={1} max={14} value={form.jobLevel} onChange={(e) => setForm({ ...form, jobLevel: Number(e.target.value) })} required /></div>
            <div className="form-group">
              <label>Employment Type</label>
              <select className="form-control" value={form.employmentType} onChange={(e) => setForm({ ...form, employmentType: Number(e.target.value) })}>
                <option value={1}>Permanent</option>
                <option value={2}>Contract</option>
                <option value={3}>Internship</option>
                <option value={4}>Casual</option>
              </select>
            </div>
            <div className="form-group">
              <label>Department</label>
              <select className="form-control" value={form.departmentId} onChange={(e) => setForm({ ...form, departmentId: e.target.value, positionId: "" })} required>
                <option value="">Select department</option>
                {assignableDepartments.map((department) => (
                  <option key={department.id} value={department.id}>{department.name}</option>
                ))}
              </select>
            </div>
            <div className="form-group">
              <label>Position (maps salary grade)</label>
              <select className="form-control" value={form.positionId} onChange={(e) => setForm({ ...form, positionId: e.target.value })}>
                <option value="">Unassigned (level fallback)</option>
                {positionsForDepartment.map((position) => (
                  <option key={position.id} value={position.id}>{position.title} ({position.jobGradeCode})</option>
                ))}
              </select>
            </div>
          </div>

          <div style={{ display: "flex", gap: 12 }}>
            <button type="submit" className="btn btn-primary" disabled={saving || !canManage}>{saving ? "Saving..." : "Save Profile"}</button>
            {!canManage && <span style={{ color: "var(--text-muted)", fontSize: 12, alignSelf: "center" }}>Only Admin/HR Manager can edit profile.</span>}
          </div>
        </form>
      )}

      {activeTab === "leave" && (
        <div className="card">
          <div className="card-header">
            <span className="card-title">Leave Requests ({leaves.length})</span>
          </div>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Type</th>
                  <th>Period</th>
                  <th>Status</th>
                  <th>Reason</th>
                  <th>Action</th>
                </tr>
              </thead>
              <tbody>
                {leaves.length === 0 ? (
                  <tr><td colSpan={5} style={{ textAlign: "center", padding: 24, color: "var(--text-muted)" }}>No leave requests found.</td></tr>
                ) : leaves.map((leave) => (
                  <tr key={leave.id}>
                    <td>{leave.leaveType}</td>
                    <td>{leave.startDate} to {leave.endDate}</td>
                    <td>{leaveStatusLabel[leave.status] ?? leave.status}</td>
                    <td>{leave.reason}</td>
                    <td>
                      {canReviewLeave && leave.status === 1 ? (
                        <div style={{ display: "flex", gap: 8 }}>
                          <button className="btn btn-sm btn-primary" onClick={() => reviewLeave(leave.id, 2)}>Approve</button>
                          <button className="btn btn-sm" style={{ borderColor: "var(--red)", color: "var(--red)" }} onClick={() => reviewLeave(leave.id, 3)}>Reject</button>
                        </div>
                      ) : (
                        <span style={{ color: "var(--text-muted)", fontSize: 12 }}>-</span>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {activeTab === "attendance" && (
        <div className="card">
          <div className="card-header">
            <span className="card-title">Attendance Overview</span>
            {attendanceRollup && <span style={{ color: "var(--text-muted)", fontSize: 12 }}>Present {attendanceRollup.presentDays} | Late {attendanceRollup.lateDays} | Hours {attendanceRollup.totalHoursWorked}</span>}
          </div>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Date</th>
                  <th>Status</th>
                  <th>Check In</th>
                  <th>Check Out</th>
                  <th>Hours</th>
                  <th>Notes</th>
                </tr>
              </thead>
              <tbody>
                {attendanceForEmployee.length === 0 ? (
                  <tr><td colSpan={6} style={{ textAlign: "center", padding: 24, color: "var(--text-muted)" }}>No attendance records found.</td></tr>
                ) : attendanceForEmployee.map((record) => (
                  <tr key={record.id}>
                    <td>{record.attendanceDate}</td>
                    <td>{record.status}</td>
                    <td>{record.checkInTime}</td>
                    <td>{record.checkOutTime ?? "-"}</td>
                    <td>{record.hoursWorked}</td>
                    <td>{record.notes ?? "-"}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {activeTab === "onboarding" && (
        <div className="card">
          <div className="card-header">
            <span className="card-title">Onboarding Records ({onboardings.length})</span>
          </div>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Start</th>
                  <th>Target Completion</th>
                  <th>Status</th>
                  <th>Progress</th>
                  <th>Action</th>
                </tr>
              </thead>
              <tbody>
                {onboardings.length === 0 ? (
                  <tr><td colSpan={5} style={{ textAlign: "center", padding: 24, color: "var(--text-muted)" }}>No onboarding records found.</td></tr>
                ) : onboardings.map((onboarding) => (
                  <tr key={onboarding.id}>
                    <td>{onboarding.startDate}</td>
                    <td>{onboarding.targetCompletionDate}</td>
                    <td>{onboardingStatusLabel[onboarding.status] ?? onboarding.status}</td>
                    <td>{onboarding.completedItems}/{onboarding.totalItems}</td>
                    <td>
                      {canManage ? (
                        <select
                          className="form-control"
                          value={onboarding.status}
                          onChange={(e) => updateOnboardingStatus(onboarding.id, Number(e.target.value))}
                          style={{ minWidth: 150 }}
                        >
                          <option value={1}>Not Started</option>
                          <option value={2}>In Progress</option>
                          <option value={3}>Completed</option>
                        </select>
                      ) : (
                        <span style={{ color: "var(--text-muted)", fontSize: 12 }}>Admin/HR only</span>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {activeTab === "payroll" && (
        <div className="card">
          <div className="card-header">
            <span className="card-title">Payroll Records ({payroll.length})</span>
          </div>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Period</th>
                  <th>Gross</th>
                  <th>Deductions</th>
                  <th>Net</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                {payroll.length === 0 ? (
                  <tr><td colSpan={5} style={{ textAlign: "center", padding: 24, color: "var(--text-muted)" }}>No payroll records available or access restricted.</td></tr>
                ) : payroll.map((record) => (
                  <tr key={record.id}>
                    <td>{record.payPeriod}</td>
                    <td>{record.grossPay.toLocaleString()}</td>
                    <td>{record.deductions.toLocaleString()}</td>
                    <td>{record.netPay.toLocaleString()}</td>
                    <td>{record.statusLabel}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {activeTab === "benefits" && (
        <div className="card">
          <div className="card-header">
            <span className="card-title">Benefit Enrollments ({benefits.length})</span>
          </div>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Plan</th>
                  <th>Status</th>
                  <th>Start</th>
                  <th>End</th>
                  <th>Employer</th>
                  <th>Employee</th>
                </tr>
              </thead>
              <tbody>
                {benefits.length === 0 ? (
                  <tr><td colSpan={6} style={{ textAlign: "center", padding: 24, color: "var(--text-muted)" }}>No benefit enrollments found.</td></tr>
                ) : benefits.map((benefit) => (
                  <tr key={benefit.id}>
                    <td>{benefit.benefitPlanName}</td>
                    <td>{benefit.statusLabel}</td>
                    <td>{benefit.startDate}</td>
                    <td>{benefit.endDate ?? "-"}</td>
                    <td>{benefit.employerContribution.toLocaleString()}</td>
                    <td>{benefit.employeeContribution.toLocaleString()}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}
