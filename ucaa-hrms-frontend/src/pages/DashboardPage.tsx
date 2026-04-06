import { useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import PageTitle from "../components/PageTitle";
import { DashboardMetrics } from "../types/models";

export default function DashboardPage() {
  const [metrics, setMetrics] = useState<DashboardMetrics | null>(null);

  useEffect(() => {
    apiClient.get<DashboardMetrics>("/dashboard/metrics").then((res) => setMetrics(res.data));
  }, []);

  return (
    <>
      <PageTitle title="Dashboard" subtitle="Role-based KPI overview" />
      <div className="kpi-grid">
        <div className="kpi-card">
          <span>Total Employees</span>
          <strong>{metrics?.totalEmployees ?? 0}</strong>
        </div>
        <div className="kpi-card">
          <span>Employees On Leave</span>
          <strong>{metrics?.employeesOnLeave ?? 0}</strong>
        </div>
        <div className="kpi-card">
          <span>Upcoming Shifts</span>
          <strong>{metrics?.upcomingShifts ?? 0}</strong>
        </div>
      </div>
    </>
  );
}
