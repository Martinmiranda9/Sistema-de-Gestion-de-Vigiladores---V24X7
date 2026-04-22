export interface KpiCard {
  title: string;
  value: string;
  subtitle: string;
  icon: string;
  lastUpdate: Date;
  color: string;
}

export interface QuickAction {
  title: string;
  description: string;
  icon: string;
  route: string;
  color: string;
}
