
import ExpenseItem from "./ExpenseItem";
import "./Expenses.css";
import Card from "../UI/Card";


const ExpenseList = (prop) => {

    return (
        <Card className="expenses">
          <ExpenseItem expense={prop.expenses[0]}></ExpenseItem>
          <ExpenseItem expense={prop.expenses[1]}></ExpenseItem>
          <ExpenseItem expense={prop.expenses[2]}></ExpenseItem>
          <ExpenseItem expense={prop.expenses[3]}></ExpenseItem>
          <ExpenseItem expense={prop.expenses[4]}></ExpenseItem>
        </Card>);
}

export default ExpenseList;