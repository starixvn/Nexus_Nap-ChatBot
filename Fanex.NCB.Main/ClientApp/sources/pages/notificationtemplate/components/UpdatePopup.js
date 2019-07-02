import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import PropTypes from 'prop-types';

export class UpdatePopup extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div></div>
        );
    }
}

UpdatePopup.propTypes = {
    closeUpdatePopupHandler: PropTypes.func.isRequired
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { settingData, updatePopupData, updateResponseData } = state;
    return {
        settingData,
        updatePopupData,
        updateResponseData
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(UpdatePopup);