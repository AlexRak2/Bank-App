async function getChartData() {
    try {
        console.log("attempt");

        const response = await fetch('/api/GetChartData', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const data = await response.json();
            fillOutChart(data);
        } else {
            console.error('Failed to fetch chart data:', response.status);
        }
    } catch (error) {
        console.error('Error fetching chart data:', error);
    }
}

function fillOutChart(data) {
    var ctx = document.getElementById('myLineChart').getContext('2d');

    var totalIncomeElement = document.getElementById('totalIncome');
    var totalExpenseElement = document.getElementById('totalExpense');
    var totalBalanceElement = document.getElementById('totalBalance');

    const expenseMonths = [];
    const expenseAmount = [];
    const incomeAmount = [];

    let totalExpenses = 0;
    let totalIncome = 0;

    data.expenseData.forEach(item => {
        expenseMonths.push(item.month);
        expenseAmount.push(item.amount);

        totalExpenses += item.amount;
    });

    data.incomeData.forEach(item => {
        incomeAmount.push(item.amount);

        totalIncome += item.amount;
    });

    let balance = totalIncome - totalExpenses;

    totalIncomeElement.textContent = "$"+totalIncome;
    totalExpenseElement.textContent = "$" + totalExpenses;
    totalBalanceElement.textContent = "$" + balance;

    var myLineChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: expenseMonths,
            datasets: [{
                label: 'Income',
                data: incomeAmount, 
                fill: false,
                borderColor: '#5cb066',
                tension: 0.1
            },
            {
                label: 'Expenses',
                data: expenseAmount, 
                fill: false,
                borderColor: '#cb3e3e',
                tension: 0.1
            }]
        }
    });
}

getChartData();

