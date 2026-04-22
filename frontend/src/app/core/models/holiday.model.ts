export interface Holiday {
  id: number;
  date: string;
  description: string;
  isRecurring: boolean;
}

export interface HolidayCreate {
  date: string;
  description: string;
  isRecurring: boolean;
}
