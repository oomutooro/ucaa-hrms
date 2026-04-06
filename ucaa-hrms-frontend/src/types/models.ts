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
  positionId?: string | null;
  positionTitle: string;
  salaryGradeCode: string;
  salaryGradeTitle: string;
  jobTitle: string;
  employmentType: number;
  annualLeaveBalanceDays: number;
}

export interface BenefitPlan {
  id: string;
  name: string;
  planType: number;
  planTypeLabel: string;
  description: string;
  isTaxable: boolean;
  isActive: boolean;
  defaultEmployerContribution: number;
  defaultEmployeeContribution: number;
  activeEnrollments: number;
}

export interface BenefitEnrollment {
  id: string;
  employeeId: string;
  employeeName: string;
  employeeNumber: string;
  employeeJobLevel: number;
  employeeJobTitle: string;
  salaryGradeCode: string;
  salaryGradeTitle: string;
  benefitPlanId: string;
  benefitPlanName: string;
  benefitPlanType: number;
  benefitPlanTypeLabel: string;
  startDate: string;
  endDate?: string | null;
  status: number;
  statusLabel: string;
  employerContribution: number;
  employeeContribution: number;
  totalContribution: number;
  notes: string;
}

export interface BenefitSummary {
  planCount: number;
  activePlanCount: number;
  enrollmentCount: number;
  activeEnrollmentCount: number;
  totalMonthlyEmployerContribution: number;
  totalMonthlyEmployeeContribution: number;
  totalMonthlyContribution: number;
  enrollmentsByPlanType: Record<string, number>;
  activeEnrollmentsBySalaryGrade: Record<string, number>;
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

export interface LeavePolicyRule {
  leaveType: number;
  leaveTypeLabel: string;
  maxDaysPerRequest: number;
}

export interface LeaveBalance {
  employeeId: string;
  employeeName: string;
  departmentName: string;
  annualLeaveEntitlementDays: number;
  annualLeaveBalanceDays: number;
  annualLeaveUsedDays: number;
}

export interface LeaveSummary {
  totalRequests: number;
  pendingRequests: number;
  approvedRequests: number;
  rejectedRequests: number;
  employeesCurrentlyOnLeave: number;
  upcomingApprovedLeaves: number;
  policyRules: LeavePolicyRule[];
  leaveBalances: LeaveBalance[];
}

export interface PayrollRecord {
  id: string;
  employeeId: string;
  employeeName: string;
  basicSalary: number;
  transportAllowance: number;
  housingAllowance: number;
  otherAllowance: number;
  allowances: number;
  payeTax: number;
  pensionDeduction: number;
  loanDeduction: number;
  otherDeduction: number;
  deductions: number;
  grossPay: number;
  netPay: number;
  payPeriod: string;
  status: number;
  statusLabel: string;
  paidAtUtc?: string | null;
  notes?: string;
}

export interface PayrollSummary {
  recordCount: number;
  draftCount: number;
  approvedCount: number;
  paidCount: number;
  totalGrossPay: number;
  totalDeductions: number;
  totalNetPay: number;
  totalPaye: number;
  totalPension: number;
  latestPayPeriod?: string | null;
}

export interface ShiftAssignment {
  id: string;
  shiftDate: string;
  shiftType: number;
  employeeId?: string;
  employeeName?: string;
  shiftCode: string;
}

export interface AttendanceRecord {
  id: string;
  employeeId: string;
  employeeName: string;
  shiftAssignmentId?: string | null;
  attendanceDate: string;
  shiftType?: number | null;
  status: number;
  checkInTime: string;
  checkOutTime?: string | null;
  hoursWorked: number;
  isOpen: boolean;
  notes?: string | null;
}

export interface EmployeeAttendanceRollup {
  employeeId: string;
  employeeName: string;
  presentDays: number;
  lateDays: number;
  totalHoursWorked: number;
}

export interface AttendanceSummary {
  scheduledToday: number;
  checkedInToday: number;
  lateToday: number;
  pendingClockOuts: number;
  totalHoursToday: number;
  upcomingAssignments: ShiftAssignment[];
  recentAttendance: AttendanceRecord[];
  monthlyRollup: EmployeeAttendanceRollup[];
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

export interface OnboardingTemplate {
  id: string;
  name: string;
  description: string;
  taskCount: number;
}

export interface OnboardingTemplateTask {
  id: string;
  templateId: string;
  title: string;
  category: string;
  isRequired: boolean;
  sortOrder: number;
}

export interface EmployeeOnboarding {
  id: string;
  employeeId: string;
  employeeName: string;
  applicationId?: string | null;
  startDate: string;
  targetCompletionDate: string;
  status: number;
  statusLabel: string;
  notes?: string | null;
  totalItems: number;
  completedItems: number;
}

export interface OnboardingDetail {
  id: string;
  employeeId: string;
  employeeName: string;
  applicationId?: string | null;
  startDate: string;
  targetCompletionDate: string;
  status: number;
  statusLabel: string;
  notes?: string | null;
  items: OnboardingItem[];
}

export interface OnboardingItem {
  id: string;
  title: string;
  category: string;
  isRequired: boolean;
  sortOrder: number;
  isCompleted: boolean;
  completedAt?: string | null;
  notes?: string | null;
}
