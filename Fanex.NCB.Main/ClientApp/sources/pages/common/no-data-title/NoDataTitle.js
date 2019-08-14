import React from 'react';
import Labels from '../Labels';
import './NoDataTitle.scss';

const noDataTitle = () => {
    return (
        <div className="no-info-row center-align">{Labels.NoInfo}</div>
    );
};

export default noDataTitle;