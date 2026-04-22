export interface PayrollConfig {
  id: number;
  normalHourRate: number;
  nightSurchargeRate: number;
  holidayHourRate: number;
  extraHourRate: number;
  validFrom: string;
  reason?: string;
  changedBy?: string;
  createdAt: string;
}

export interface PayrollConfigCreate {
  normalHourRate: number;
  nightSurchargeRate: number;
  holidayHourRate: number;
  extraHourRate: number;
  validFrom: string;
  reason?: string;
  changedBy?: string;
}
