export interface Workplace {
  id: number;
  name: string;
  address: string;
  isActive: boolean;
}

export interface WorkplaceCreate {
  name: string;
  address: string;
}

export interface WorkplaceUpdate {
  name: string;
  address: string;
  isActive: boolean;
}
