export interface MenuItem {
  label: string;
  icon: string;
  route: string;
  badge?: number;
}

export interface MenuSection {
  title: string;
  items: MenuItem[];
}
