import React from 'react';
import Labels from '../Labels';
import './NoDataTitle.scss';

const NoDataTitle = () => {
    return (
        <p className="no-info-row center-align">{Labels.NoInfo}</p>
    );
};

export default NoDataTitle;