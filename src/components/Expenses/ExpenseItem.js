import "./ExpenseItem.css";
import ExpenseDate from "./ExpenseDate";
import Card from "../UI/Card";
import { useState } from "react";

function ExpenseItem(props) {

  const [title, setTitle] = useState(props.expense.title);

  const clickHandler = (event) => {
    setTitle('Update');
    console.log(title);
  };

  return (
    <Card className="expense-item">
      {<ExpenseDate date={props.expense.date}></ExpenseDate>}
      <div className="expense-item__description">
        <h2>{title}</h2>
        <div className="expense-item__price">${props.expense.amount}</div>
      </div>
      {/* <button onClick={clickHandler}>Change Title</button> */}
    </Card>
  );
}

export default ExpenseItem;
