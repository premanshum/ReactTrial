import ExpenseList from "./components/Expenses/ExpenseList";
import NewExpense from "./components/NewExpense/NewExpense";
import { useState } from "react";

let DUMMY_EXPENSES = [
{
    id: "e1",
    title: "Toilet Paper",
    amount: 94.12,
    date: "2022-06-17T09:45:40",
  },
  { id: "e2", title: "New TV", amount: 799.49, date: "2021-06-13T19:03:40" },
  {
    id: "e3",
    title: "Car Insurance",
    amount: 294.67,
    date: "2022-06-25T17:55:00",
  },
  {
    id: "e4",
    title: "New Desk (Wooden)",
    amount: 450,
    date: "2021-06-08T09:25:00",
  },
  {
    id: "e5",
    title: "Cycling gloves",
    amount: 20,
    date: "2019-06-23T17:40:00",
  }
];

function App() {

  let selYear = 0;
  const [expenses, setExpenses] = useState(DUMMY_EXPENSES);

  const addExpenseHandler = (expense) => {
    setExpenses((prevState)=>{
      return [
        expense,
        ...prevState
      ]
    });
    console.log("Inside App");
    console.log(expenses);
  };

  const filterHandler = (selectedYear) =>{
    selYear = selectedYear;
    console.log("Inside App:", selectedYear);
    const filteredExpense = expenses.filter(item => (new Date(item.date)).getFullYear().toString() === selYear.toString());
    console.log('filteredExpense :', filteredExpense);
    setExpenses(filteredExpense);
  }

  return (
    <div>
      <NewExpense onAddExpense = {addExpenseHandler}></NewExpense>
      <ExpenseList expenses={expenses} onFilter={filterHandler}></ExpenseList>
    </div>
  );
}

export default App;
