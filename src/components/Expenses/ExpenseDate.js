import "./ExpenseDate.css";

function ExpenseDate(prop) {
  //const date = prop.date;
  const date = new Date(prop.date);

  const month = date.toLocaleString("en-US", { month: "long" });
  const day = date.toLocaleString("en-US", { day: "2-digit" });
  const year = date.getFullYear();
  const time = formatAMPM(date);

  return (
    <div className="expense-date">
      <div className="expense-date__month">{month}-{year}</div>
      <div className="expense-date__day">{day}</div>
      <div className="expense-date__time">{time}</div>      
    </div>
  );
}

function formatAMPM(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0'+minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
  }

export default ExpenseDate;
