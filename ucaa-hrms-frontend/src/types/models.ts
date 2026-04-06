export interface AuthResponse {
  accessToken: string;
  email: string;
  role: string;
}

export interface DashboardMetrics {
  totalEmployees: number;
  employeesOnLeave: number;
  upcomingShifts: number;
}

export interface Department {
  id: string;
  name: string;
  parentDepartmentId?: string | null;
}

export interface Employee {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  employeeId: string;
  dateOfBirth: string;
  firstEmploymentDate: string;
  jobLevel: number;
  annualLeaveEntitlementDays: number;
  age: number;
  yearsOfService: number;
  mandatoryRetirementDate: string;
  isEligibleForVoluntaryRetirement: boolean;
  isAtOrAboveMandatoryRetirementAge: boolean;
  isEligibleForGoldenHandshake: boolean;
  isEligibleForLongServiceAward: boolean;
  noticePeriodMonths: number;
  serviceGratuityMonthsPerCompletedYear: number;
  serviceGratuityTotalMonths: number;
  redundancySeveranceMonths: number;
  departmentId: string;
  departmentName: string;
  jobTitle: string;
  employmentType: number;
  annualLeaveBalanceDays: number;
}

export interface LeaveRequest {
  id: string;
  employeeId: string;
  employeeName: string;
  leaveType: number;
  status: number;
  startDate: string;
  endDate: string;
  requestedDays: number;
  sickLeavePayPercent?: number;
  reason: string;
  reviewerComment?: string;
}

export interface PayrollRecord {
  id: string;
  employeeId: string;
  employeeName: string;
  basicSalary: number;
  allowances: number;
  deductions: number;
  netPay: number;
  payPeriod: string;
  notes?: string;
}

export interface ShiftAssignment {
  id: string;
  shiftDate: string;
  shiftType: number;
  employeeId?: string;
  employeeName?: string;
  shiftCode: string;
}

export interface JobGrade {
  id: string;
  gradeCode: string;
  gradeTitle: string;
  minSalary: number;
  maxSalary: number;
}

export interface JobDescription {
  id: string;
  title: string;
  purposeStatement: string;
  keyAccountabilities: string;
  qualifications: string;
  jobGradeId: string;
  jobGradeCode: string;
  jobGradeTitle: string;
}

export interface Position {
  id: string;
  title: string;
  departmentId: string;
  departmentName: string;
  jobDescriptionId: string;
  jobDescriptionTitle: string;
  jobGradeCode: string;
  approvedHeadcount: number;
}

export interface JobRequisition {
  id: string;
  requisitionNumber: string;
  positionId: string;
  positionTitle: string;
  departmentId: string;
  departmentName: string;
  vacanciesRequested: number;
  justification: string;
  closingDate: string;
  status: number;
  statusLabel: string;
  applicationCount: number;
}

export interface JobApplication {
  id: string;
  requisitionId: string;
  requisitionNumber: string;
  positionTitle: string;
  applicantName: string;
  applicantEmail: string;
  applicantPhone: string;
  isInternal: boolean;
  employeeId?: string | null;
  status: number;
  statusLabel: string;
  reviewNotes?: string | null;
  interviewDate?: string | null;
  appliedAtUtc: string;
}

export interface DocumentItem {
  id: string;
  employeeId?: string;
  documentType: number;
  fileName: string;
  contentType: string;
  fileSizeBytes: number;
}
