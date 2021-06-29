import React, { useState } from "react";
import ExpenseItem from "./ExpenseItem";
import "./Expenses.css";
import Card from "../UI/Card";
import ExpensesFilter from "./ExpensesFilter";


const ExpenseList = (props) => {
  
  const [filteredYear, setFilteredYear] = useState("2020");
  const [expenses, setExpenses] = useState(props.expenses);

  const filterChangeHandler = (selectedYear)=>{
      setFilteredYear(selectedYear);
      console.log("In ExpenseList filterChangeHandler: ", selectedYear);
      props.onFilter(selectedYear);
      //setExpenses(props.expenses);
  }

  console.log("In ExpenseList:", expenses);

  return (
    <div>
      <Card className="expenses">
        <ExpensesFilter onFilterChange ={filterChangeHandler} selected={filteredYear} />
        {props.expenses.map( item => (<ExpenseItem expense={item} key={item.id}></ExpenseItem>))}
      </Card>
    </div>);
}

export default ExpenseList;