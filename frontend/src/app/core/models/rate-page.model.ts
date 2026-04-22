export interface TimelineEvent {
  validFrom: Date;
  createdAt: Date;
  rate: number;
  reason?: string;
  changedBy?: string;
  id: number;
  isCurrent: boolean;
  isUpcoming: boolean;
}
