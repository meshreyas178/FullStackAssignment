import { Component, OnInit } from '@angular/core';
import { EmployeeService } from '../employee.service';
import { Chart, registerables } from 'chart.js';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.css']
})
export class EmployeeListComponent implements OnInit {
  employees: any[] = [];
  selectedEmployee: any;
  totalHours: number = 0;
  chart: any;

  constructor(private employeeService: EmployeeService) {
    Chart.register(...registerables);
  }

  ngOnInit(): void {
    this.employeeService.getTimeEntries().subscribe(data => {
      this.employees = data;
      this.totalHours = this.employees.reduce((acc, emp) => acc + emp.hoursWorked, 0);
    });
  }

  updateChart(employee: any): void {
    this.selectedEmployee = employee;
    const percentage = (employee.hoursWorked / this.totalHours) * 100;

    if (this.chart) {
      this.chart.destroy();
    }

    this.chart = new Chart('pieChart', {
      type: 'pie',
      data: {
        labels: ['Worked Hours', 'Remaining Hours'],
        datasets: [{
          data: [percentage, 100 - percentage],
          backgroundColor: ['#36A2EB', '#FF6384']
        }]
      }
    });
  }
}
