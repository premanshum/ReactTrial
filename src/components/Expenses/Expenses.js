import React, { useState } from "react";
import ExpensesFilter from "./ExpensesFilter";
import "./Expenses.css";
import Card from "../UI/Card";
import ExpensesList from "./ExpensesList";

const Expenses = (props) => {
  const [filteredYear, setFilteredYear] = useState("2020");

  const filterChangeHandler = (selectedYear) => {
    setFilteredYear(selectedYear);
  };

  const filteredExpense = props.expenses.filter(
    (item) =>
      new Date(item.date).getFullYear().toString() === filteredYear.toString()
  );
  console.log(filteredExpense);

  return (
    <div>
      <Card className='expenses'>
        <ExpensesFilter
          selectedYear = {filteredYear}
          onFilterChange = {filterChangeHandler}
        />
      </Card>
      <ExpensesList items={filteredExpense} />
    </div>
  );
};

export default Expenses;
