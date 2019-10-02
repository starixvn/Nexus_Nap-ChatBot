import React from 'react';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import './DatePicker.scss';

export default function (props) {
    const { className, ...rest } = props;
    return (
        <DatePicker
            className={`form-control datetime-picker ${className}`}
            dateFormat={props.showTimeSelect ? 'MM/dd/yyyy hh:mm aa' : 'MM/dd/yyyy'}
            timeFormat="hh:mm aa"
            timeIntervals={5}
            {...rest}
        />
    );
};