import { FormEvent, useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import { BenefitEnrollment, BenefitPlan, BenefitSummary, Employee } from "../types/models";

const money = (value: number) =>
  new Intl.NumberFormat("en-UG", { style: "currency", currency: "UGX", maximumFractionDigits: 0 }).format(value);

export default function BenefitsPage() {
  const [plans, setPlans] = useState<BenefitPlan[]>([]);
  const [enrollments, setEnrollments] = useState<BenefitEnrollment[]>([]);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [summary, setSummary] = useState<BenefitSummary | null>(null);
  const [error, setError] = useState<string | null>(null);

  const [planForm, setPlanForm] = useState({
    name: "",
    planType: 1,
    description: "",
    isTaxable: false,
    defaultEmployerContribution: 0,
    defaultEmployeeContribution: 0,
  });

  const [enrollmentForm, setEnrollmentForm] = useState({
    employeeId: "",
    benefitPlanId: "",
    startDate: new Date().toISOString().slice(0, 10),
    endDate: "",
    employerContribution: "",
    employeeContribution: "",
    notes: "",
  });

  const load = () => {
    Promise.all([
      apiClient.get<BenefitPlan[]>("/benefits/plans"),
      apiClient.get<BenefitEnrollment[]>("/benefits/enrollments"),
      apiClient.get<BenefitSummary>("/benefits/summary"),
      apiClient.get<Employee[]>("/employees"),
    ])
      .then(([p, e, s, em]) => {
        setPlans(p.data);
        setEnrollments(e.data);
        setSummary(s.data);
        setEmployees(em.data);
        setError(null);
      })
      .catch(() => setError("Failed to load benefits data."));
  };

  useEffect(() => {
    load();
  }, []);

  const onCreatePlan = async (event: FormEvent) => {
    event.preventDefault();
    await apiClient.post("/benefits/plans", {
      ...planForm,
      planType: Number(planForm.planType),
      defaultEmployerContribution: Number(planForm.defaultEmployerContribution),
      defaultEmployeeContribution: Number(planForm.defaultEmployeeContribution),
    });
    setPlanForm({
      name: "",
      planType: 1,
      description: "",
      isTaxable: false,
      defaultEmployerContribution: 0,
      defaultEmployeeContribution: 0,
    });
    load();
  };

  const onCreateEnrollment = async (event: FormEvent) => {
    event.preventDefault();
    await apiClient.post("/benefits/enrollments", {
      employeeId: enrollmentForm.employeeId,
      benefitPlanId: enrollmentForm.benefitPlanId,
      startDate: enrollmentForm.startDate,
      endDate: enrollmentForm.endDate || null,
      employerContribution: enrollmentForm.employerContribution ? Number(enrollmentForm.employerContribution) : null,
      employeeContribution: enrollmentForm.employeeContribution ? Number(enrollmentForm.employeeContribution) : null,
      notes: enrollmentForm.notes || null,
    });
    setEnrollmentForm({
      employeeId: "",
      benefitPlanId: "",
      startDate: new Date().toISOString().slice(0, 10),
      endDate: "",
      employerContribution: "",
      employeeContribution: "",
      notes: "",
    });
    load();
  };

  const updateEnrollmentStatus = async (id: string, status: number) => {
    const today = new Date().toISOString().slice(0, 10);
    await apiClient.patch(`/benefits/enrollments/${id}/status`, {
      status,
      endDate: status === 3 ? today : null,
      notes: status === 2 ? "Suspended by HR" : status === 3 ? "Terminated by HR" : "Reactivated by HR",
    });
    load();
  };

  return (
    <div>
      <div className="page-title-block">
        <h2>Benefits Administration</h2>
        <p>Manage benefit plans, enrollments, and contribution exposure by salary grade</p>
      </div>

      {error && (
        <div className="card" style={{ color: "var(--red)", marginBottom: 16 }}>
          {error}
        </div>
      )}

      {summary && (
        <div className="stats-grid" style={{ marginBottom: 16 }}>
          <div className="stats-card">
            <span className="stats-label">Plans</span>
            <span className="stats-value">{summary.activePlanCount} / {summary.planCount}</span>
            <span className="stats-sub">Active / Total</span>
          </div>
          <div className="stats-card">
            <span className="stats-label">Enrollments</span>
            <span className="stats-value">{summary.activeEnrollmentCount} / {summary.enrollmentCount}</span>
            <span className="stats-sub">Active / Total</span>
          </div>
          <div className="stats-card">
            <span className="stats-label">Employer Cost</span>
            <span className="stats-value">{money(summary.totalMonthlyEmployerContribution)}</span>
            <span className="stats-sub">Monthly</span>
          </div>
          <div className="stats-card">
            <span className="stats-label">Total Cost</span>
            <span className="stats-value">{money(summary.totalMonthlyContribution)}</span>
            <span className="stats-sub">Employer + Employee monthly</span>
          </div>
        </div>
      )}

      <div className="content-grid" style={{ marginBottom: 16 }}>
        <form className="card" onSubmit={onCreatePlan} style={{ display: "flex", flexDirection: "column", gap: 10 }}>
          <div className="card-header" style={{ marginBottom: 4 }}>
            <span className="card-title">Create Benefit Plan</span>
          </div>
          <div className="form-group">
            <label>Name</label>
            <input className="form-control" value={planForm.name} onChange={(e) => setPlanForm({ ...planForm, name: e.target.value })} required />
          </div>
          <div className="form-group">
            <label>Type</label>
            <select className="form-control" value={planForm.planType} onChange={(e) => setPlanForm({ ...planForm, planType: Number(e.target.value) })}>
              <option value={1}>Medical</option>
              <option value={2}>Pension</option>
              <option value={3}>Life Insurance</option>
              <option value={4}>Transport</option>
              <option value={5}>Meal</option>
              <option value={6}>Housing</option>
              <option value={7}>Education</option>
              <option value={99}>Other</option>
            </select>
          </div>
          <div className="form-group">
            <label>Description</label>
            <input className="form-control" value={planForm.description} onChange={(e) => setPlanForm({ ...planForm, description: e.target.value })} required />
          </div>
          <div className="form-group">
            <label>Default Employer Contribution</label>
            <input className="form-control" type="number" min={0} step="0.01" value={planForm.defaultEmployerContribution} onChange={(e) => setPlanForm({ ...planForm, defaultEmployerContribution: Number(e.target.value) })} />
          </div>
          <div className="form-group">
            <label>Default Employee Contribution</label>
            <input className="form-control" type="number" min={0} step="0.01" value={planForm.defaultEmployeeContribution} onChange={(e) => setPlanForm({ ...planForm, defaultEmployeeContribution: Number(e.target.value) })} />
          </div>
          <label style={{ display: "flex", alignItems: "center", gap: 8, fontSize: 13 }}>
            <input type="checkbox" checked={planForm.isTaxable} onChange={(e) => setPlanForm({ ...planForm, isTaxable: e.target.checked })} />
            Taxable benefit
          </label>
          <button className="btn btn-primary" type="submit">Create Plan</button>
        </form>

        <form className="card" onSubmit={onCreateEnrollment} style={{ display: "flex", flexDirection: "column", gap: 10 }}>
          <div className="card-header" style={{ marginBottom: 4 }}>
            <span className="card-title">Enroll Employee</span>
          </div>
          <div className="form-group">
            <label>Employee</label>
            <select className="form-control" value={enrollmentForm.employeeId} onChange={(e) => setEnrollmentForm({ ...enrollmentForm, employeeId: e.target.value })} required>
              <option value="">Select employee</option>
              {employees.map((e) => (
                <option key={e.id} value={e.id}>{e.fullName} ({e.salaryGradeCode})</option>
              ))}
            </select>
          </div>
          <div className="form-group">
            <label>Benefit Plan</label>
            <select className="form-control" value={enrollmentForm.benefitPlanId} onChange={(e) => setEnrollmentForm({ ...enrollmentForm, benefitPlanId: e.target.value })} required>
              <option value="">Select plan</option>
              {plans.filter((p) => p.isActive).map((p) => (
                <option key={p.id} value={p.id}>{p.name} ({p.planTypeLabel})</option>
              ))}
            </select>
          </div>
          <div className="form-group">
            <label>Start Date</label>
            <input className="form-control" type="date" value={enrollmentForm.startDate} onChange={(e) => setEnrollmentForm({ ...enrollmentForm, startDate: e.target.value })} required />
          </div>
          <div className="form-group">
            <label>End Date (optional)</label>
            <input className="form-control" type="date" value={enrollmentForm.endDate} onChange={(e) => setEnrollmentForm({ ...enrollmentForm, endDate: e.target.value })} />
          </div>
          <div className="form-group">
            <label>Employer Contribution (optional override)</label>
            <input className="form-control" type="number" min={0} step="0.01" value={enrollmentForm.employerContribution} onChange={(e) => setEnrollmentForm({ ...enrollmentForm, employerContribution: e.target.value })} />
          </div>
          <div className="form-group">
            <label>Employee Contribution (optional override)</label>
            <input className="form-control" type="number" min={0} step="0.01" value={enrollmentForm.employeeContribution} onChange={(e) => setEnrollmentForm({ ...enrollmentForm, employeeContribution: e.target.value })} />
          </div>
          <div className="form-group">
            <label>Notes</label>
            <input className="form-control" value={enrollmentForm.notes} onChange={(e) => setEnrollmentForm({ ...enrollmentForm, notes: e.target.value })} />
          </div>
          <button className="btn btn-primary" type="submit">Create Enrollment</button>
        </form>
      </div>

      <div className="card" style={{ marginBottom: 16 }}>
        <div className="card-header">
          <span className="card-title">Benefit Plans</span>
        </div>
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Name</th>
                <th>Type</th>
                <th>Default Employer</th>
                <th>Default Employee</th>
                <th>Tax</th>
                <th>Status</th>
                <th>Active Enrollments</th>
              </tr>
            </thead>
            <tbody>
              {plans.length === 0 ? (
                <tr><td colSpan={7} style={{ textAlign: "center", color: "var(--text-muted)", padding: 24 }}>No benefit plans found</td></tr>
              ) : plans.map((p) => (
                <tr key={p.id}>
                  <td>{p.name}</td>
                  <td>{p.planTypeLabel}</td>
                  <td>{money(p.defaultEmployerContribution)}</td>
                  <td>{money(p.defaultEmployeeContribution)}</td>
                  <td>{p.isTaxable ? "Taxable" : "Non-taxable"}</td>
                  <td>{p.isActive ? "Active" : "Inactive"}</td>
                  <td>{p.activeEnrollments}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <div className="card">
        <div className="card-header">
          <span className="card-title">Employee Enrollments</span>
        </div>
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Employee</th>
                <th>Grade</th>
                <th>Plan</th>
                <th>Start</th>
                <th>End</th>
                <th>Status</th>
                <th>Employer</th>
                <th>Employee</th>
                <th>Total</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {enrollments.length === 0 ? (
                <tr><td colSpan={10} style={{ textAlign: "center", color: "var(--text-muted)", padding: 24 }}>No enrollments found</td></tr>
              ) : enrollments.map((e) => (
                <tr key={e.id}>
                  <td>
                    <div style={{ fontWeight: 700 }}>{e.employeeName}</div>
                    <div style={{ color: "var(--text-muted)", fontSize: 11 }}>{e.employeeNumber} | L{e.employeeJobLevel}</div>
                  </td>
                  <td>
                    <div style={{ fontWeight: 700 }}>{e.salaryGradeCode}</div>
                    <div style={{ color: "var(--text-muted)", fontSize: 11 }}>{e.salaryGradeTitle}</div>
                  </td>
                  <td>{e.benefitPlanName}</td>
                  <td>{e.startDate}</td>
                  <td>{e.endDate || "-"}</td>
                  <td>{e.statusLabel}</td>
                  <td>{money(e.employerContribution)}</td>
                  <td>{money(e.employeeContribution)}</td>
                  <td>{money(e.totalContribution)}</td>
                  <td>
                    <div style={{ display: "flex", gap: 8 }}>
                      <button className="btn" type="button" onClick={() => updateEnrollmentStatus(e.id, 1)} style={{ padding: "4px 10px" }}>Activate</button>
                      <button className="btn" type="button" onClick={() => updateEnrollmentStatus(e.id, 2)} style={{ padding: "4px 10px" }}>Suspend</button>
                      <button className="btn" type="button" onClick={() => updateEnrollmentStatus(e.id, 3)} style={{ padding: "4px 10px", borderColor: "var(--red)", color: "var(--red)" }}>Terminate</button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
