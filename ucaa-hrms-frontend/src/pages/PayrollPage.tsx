import { FormEvent, useMemo, useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import { Employee, PayrollRecord, PayrollSummary } from "../types/models";

const statusColors: Record<number, { bg: string; fg: string }> = {
  1: { bg: "#fff3cd", fg: "#856404" },
  2: { bg: "#d1ecf1", fg: "#0c5460" },
  3: { bg: "#d4edda", fg: "#155724" },
};

function MetricCard({ label, value, helper }: { label: string; value: number | string; helper: string }) {
  return (
    <div className="card" style={{ padding: 18 }}>
      <div style={{ color: "#666", fontSize: 12 }}>{label}</div>
      <div style={{ fontSize: 28, fontWeight: 700, marginTop: 6 }}>{value}</div>
      <div style={{ color: "#8b8b8b", fontSize: 12, marginTop: 4 }}>{helper}</div>
    </div>
  );
}

export default function PayrollPage() {
  const [records, setRecords] = useState<PayrollRecord[]>([]);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [summary, setSummary] = useState<PayrollSummary | null>(null);
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [form, setForm] = useState({
    employeeId: "",
    basicSalary: 0,
    transportAllowance: 0,
    housingAllowance: 0,
    otherAllowance: 0,
    payeTax: 0,
    pensionDeduction: 0,
    loanDeduction: 0,
    otherDeduction: 0,
    payPeriod: "",
    notes: "",
  });
  const [filter, setFilter] = useState({ payPeriod: "", status: "all", search: "" });

  const allowancesTotal = useMemo(
    () => Number(form.transportAllowance) + Number(form.housingAllowance) + Number(form.otherAllowance),
    [form.transportAllowance, form.housingAllowance, form.otherAllowance]
  );
  const deductionsTotal = useMemo(
    () => Number(form.payeTax) + Number(form.pensionDeduction) + Number(form.loanDeduction) + Number(form.otherDeduction),
    [form.payeTax, form.pensionDeduction, form.loanDeduction, form.otherDeduction]
  );
  const grossPay = Number(form.basicSalary) + allowancesTotal;
  const netPay = grossPay - deductionsTotal;

  const load = async () => {
    setIsLoading(true);
    try {
      const [recordsResponse, employeesResponse, summaryResponse] = await Promise.all([
        apiClient.get<PayrollRecord[]>("/payroll"),
        apiClient.get<Employee[]>("/employees"),
        apiClient.get<PayrollSummary>("/payroll/summary"),
      ]);

      setRecords(recordsResponse.data);
      setEmployees(employeesResponse.data);
      setSummary(summaryResponse.data);
      setError("");
    } catch (requestError: any) {
      setError(requestError.response?.data?.message ?? "Failed to load payroll data.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    try {
      await apiClient.post("/payroll", {
        ...form,
        basicSalary: Number(form.basicSalary),
        transportAllowance: Number(form.transportAllowance),
        housingAllowance: Number(form.housingAllowance),
        otherAllowance: Number(form.otherAllowance),
        payeTax: Number(form.payeTax),
        pensionDeduction: Number(form.pensionDeduction),
        loanDeduction: Number(form.loanDeduction),
        otherDeduction: Number(form.otherDeduction),
      });

      setForm({
        employeeId: "",
        basicSalary: 0,
        transportAllowance: 0,
        housingAllowance: 0,
        otherAllowance: 0,
        payeTax: 0,
        pensionDeduction: 0,
        loanDeduction: 0,
        otherDeduction: 0,
        payPeriod: "",
        notes: "",
      });
      await load();
    } catch (requestError: any) {
      setError(requestError.response?.data?.message ?? "Failed to create payroll record.");
    }
  };

  const onUpdateStatus = async (recordId: string, status: number) => {
    try {
      await apiClient.patch(`/payroll/${recordId}/status`, { status });
      await load();
    } catch (requestError: any) {
      setError(requestError.response?.data?.message ?? "Failed to update payroll status.");
    }
  };

  const filteredRecords = useMemo(
    () => records.filter((record) => {
      const matchesPeriod = !filter.payPeriod || record.payPeriod.startsWith(filter.payPeriod);
      const matchesStatus = filter.status === "all" || record.status === Number(filter.status);
      const matchesSearch =
        !filter.search ||
        record.employeeName.toLowerCase().includes(filter.search.toLowerCase()) ||
        (record.notes ?? "").toLowerCase().includes(filter.search.toLowerCase());
      return matchesPeriod && matchesStatus && matchesSearch;
    }),
    [records, filter]
  );

  return (
    <div style={{ padding: "24px 32px" }}>
      <div className="page-title-block">
        <h2>Payroll Engine</h2>
        <p>Component-based payroll processing with approval and payment workflow.</p>
      </div>

      {error && <div className="card" style={{ marginBottom: 16, borderLeft: "4px solid #dc3545", color: "#721c24" }}>{error}</div>}

      <div style={{ display: "grid", gridTemplateColumns: "repeat(5, minmax(0, 1fr))", gap: 16, marginBottom: 24 }}>
        <MetricCard label="Payroll Records" value={summary?.recordCount ?? 0} helper="All periods" />
        <MetricCard label="Total Gross" value={(summary?.totalGrossPay ?? 0).toLocaleString()} helper="UGX aggregate" />
        <MetricCard label="Total Deductions" value={(summary?.totalDeductions ?? 0).toLocaleString()} helper="UGX aggregate" />
        <MetricCard label="Total Net Pay" value={(summary?.totalNetPay ?? 0).toLocaleString()} helper="UGX aggregate" />
        <MetricCard label="Paid Records" value={summary?.paidCount ?? 0} helper="Status = Paid" />
      </div>

      <div className="content-grid">
        <form className="card" onSubmit={onSubmit} style={{ display: "flex", flexDirection: "column", gap: 12 }}>
          <div className="card-header" style={{ marginBottom: 4 }}>
            <span className="card-title">Create Payroll Record</span>
          </div>
          <div className="form-group">
            <label>Employee</label>
            <select className="form-control" value={form.employeeId} onChange={(e) => setForm({ ...form, employeeId: e.target.value })} required>
              <option value="">Select employee</option>
              {employees.map((employee) => (
                <option key={employee.id} value={employee.id}>{employee.fullName}</option>
              ))}
            </select>
          </div>
          <div className="form-group">
            <label>Basic Salary (UGX)</label>
            <input className="form-control" type="number" placeholder="0" value={form.basicSalary} onChange={(e) => setForm({ ...form, basicSalary: Number(e.target.value) })} />
          </div>
          <div className="form-group">
            <label>Transport Allowance (UGX)</label>
            <input className="form-control" type="number" placeholder="0" value={form.transportAllowance} onChange={(e) => setForm({ ...form, transportAllowance: Number(e.target.value) })} />
          </div>
          <div className="form-group">
            <label>Housing Allowance (UGX)</label>
            <input className="form-control" type="number" placeholder="0" value={form.housingAllowance} onChange={(e) => setForm({ ...form, housingAllowance: Number(e.target.value) })} />
          </div>
          <div className="form-group">
            <label>Other Allowance (UGX)</label>
            <input className="form-control" type="number" placeholder="0" value={form.otherAllowance} onChange={(e) => setForm({ ...form, otherAllowance: Number(e.target.value) })} />
          </div>
          <div className="form-group">
            <label>PAYE Tax (UGX)</label>
            <input className="form-control" type="number" placeholder="0" value={form.payeTax} onChange={(e) => setForm({ ...form, payeTax: Number(e.target.value) })} />
          </div>
          <div className="form-group">
            <label>Pension/NSSF (UGX)</label>
            <input className="form-control" type="number" placeholder="0" value={form.pensionDeduction} onChange={(e) => setForm({ ...form, pensionDeduction: Number(e.target.value) })} />
          </div>
          <div className="form-group">
            <label>Loan Deduction (UGX)</label>
            <input className="form-control" type="number" placeholder="0" value={form.loanDeduction} onChange={(e) => setForm({ ...form, loanDeduction: Number(e.target.value) })} />
          </div>
          <div className="form-group">
            <label>Other Deduction (UGX)</label>
            <input className="form-control" type="number" placeholder="0" value={form.otherDeduction} onChange={(e) => setForm({ ...form, otherDeduction: Number(e.target.value) })} />
          </div>
          <div className="form-group">
            <label>Pay Period</label>
            <input className="form-control" type="date" value={form.payPeriod} onChange={(e) => setForm({ ...form, payPeriod: e.target.value })} required />
          </div>
          <div className="form-group">
            <label>Notes</label>
            <textarea className="form-control" placeholder="Optional notes..." value={form.notes} onChange={(e) => setForm({ ...form, notes: e.target.value })} />
          </div>

          <div className="card" style={{ padding: 12, background: "#f8fbff" }}>
            <div style={{ fontSize: 12, color: "#666", marginBottom: 6 }}>Computation Preview</div>
            <div style={{ display: "grid", gridTemplateColumns: "repeat(3, minmax(0, 1fr))", gap: 8, fontSize: 13 }}>
              <div><strong>Gross</strong><br />{grossPay.toLocaleString()}</div>
              <div><strong>Deductions</strong><br />{deductionsTotal.toLocaleString()}</div>
              <div><strong>Net</strong><br />{netPay.toLocaleString()}</div>
            </div>
          </div>

          <button type="submit" className="btn btn-primary">Save Record</button>
        </form>

        <div className="card wide">
          <div className="card-header">
            <span className="card-title">Payroll Records</span>
            <div style={{ display: "flex", gap: 8 }}>
              <input
                className="form-control"
                style={{ width: 180 }}
                type="month"
                value={filter.payPeriod}
                onChange={(e) => setFilter({ ...filter, payPeriod: e.target.value })}
              />
              <select className="form-control" value={filter.status} onChange={(e) => setFilter({ ...filter, status: e.target.value })}>
                <option value="all">All Statuses</option>
                <option value="1">Draft</option>
                <option value="2">Approved</option>
                <option value="3">Paid</option>
              </select>
              <input
                className="form-control"
                style={{ width: 220 }}
                placeholder="Search employee or note"
                value={filter.search}
                onChange={(e) => setFilter({ ...filter, search: e.target.value })}
              />
            </div>
          </div>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Employee</th>
                  <th>Period</th>
                  <th>Gross</th>
                  <th>Deductions</th>
                  <th>Net Pay</th>
                  <th>Status</th>
                  <th>Action</th>
                </tr>
              </thead>
              <tbody>
                {!isLoading && filteredRecords.length === 0 ? (
                  <tr><td colSpan={8} style={{ textAlign: "center", color: "var(--text-muted)", padding: 24 }}>No payroll records found</td></tr>
                ) : filteredRecords.map((record) => (
                  <tr key={record.id}>
                    <td>
                      <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
                        <div className="emp-avatar" style={{ width: 30, height: 30, fontSize: 12 }}>{record.employeeName[0]}</div>
                        {record.employeeName}
                      </div>
                    </td>
                    <td>{record.payPeriod}</td>
                    <td>{record.grossPay.toLocaleString()}</td>
                    <td>{record.deductions.toLocaleString()}</td>
                    <td style={{ fontWeight: 700, color: "var(--green)" }}>{record.netPay.toLocaleString()}</td>
                    <td>
                      <span style={{
                        background: statusColors[record.status]?.bg ?? "#f1f3f5",
                        color: statusColors[record.status]?.fg ?? "#495057",
                        padding: "3px 10px",
                        borderRadius: 50,
                        fontSize: 12,
                        fontWeight: 600,
                      }}>{record.statusLabel}</span>
                    </td>
                    <td>
                      {record.status === 1 ? (
                        <button type="button" className="btn btn-sm btn-primary" onClick={() => onUpdateStatus(record.id, 2)}>Approve</button>
                      ) : record.status === 2 ? (
                        <button type="button" className="btn btn-sm btn-green" onClick={() => onUpdateStatus(record.id, 3)}>Mark Paid</button>
                      ) : (
                        <span style={{ color: "#8b8b8b", fontSize: 12 }}>{record.paidAtUtc ? new Date(record.paidAtUtc).toLocaleDateString() : "Paid"}</span>
                      )}
                    </td>
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
