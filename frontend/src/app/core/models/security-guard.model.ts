export interface SecurityGuard {
  id: number;
  firstName: string;
  lastName: string;
  dni: string;
  fileNumber: string;
  workplaceId: number | null;
  workplaceName: string | null;
  isActive: boolean;
  fullName: string;
}

export interface SecurityGuardCreate {
  firstName: string;
  lastName: string;
  dni: string;
  fileNumber: string;
  workplaceId: number | null;
}

export interface SecurityGuardUpdate {
  firstName: string;
  lastName: string;
  dni: string;
  fileNumber: string;
  workplaceId: number | null;
  isActive: boolean;
}
