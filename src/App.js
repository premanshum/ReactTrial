import ExpenseList from "./components/Expenses/ExpenseList";

const expenses = [
  {
    id: "e1",
    title: "Toilet Paper",
    amount: 94.12,
    date: "2021-06-17T09:45:40",
  },
  { id: "e2", title: "New TV", amount: 799.49, date: "2021-06-13T19:03:40" },
  {
    id: "e3",
    title: "Car Insurance",
    amount: 294.67,
    date: "2021-06-25T17:55:00",
  },
  {
    id: "e4",
    title: "New Desk (Wooden)",
    amount: 450,
    date: "2021-06-08T09:25:00",
  },
  {
    id: "e4",
    title: "Cycling gloves",
    amount: 20,
    date: "2021-06-23T17:40:00",
  }
];

function App() {
  return (
    <div>
      <h2>Let's get started!</h2>
      <ExpenseList expenses={expenses}></ExpenseList>
    </div>
  );
}

export default App;
