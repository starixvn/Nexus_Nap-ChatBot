import React from 'react';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import "./DatePicker.scss";

export default function DatePickerHOC(props) {
    return (
        <DatePicker
            className="form-control datetime-picker"
            dateFormat={props.showTimeSelect ? 'MM/dd/yyyy hh:mm aa' : 'MM/dd/yyyy'}
            timeFormat="HH:mm"
            timeIntervals={5}
            {...props}
        />
    );
}