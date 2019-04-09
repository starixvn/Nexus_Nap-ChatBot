import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Modal, ModalHeader, ModalBody } from 'reactstrap';
import { Form, FormGroup, Label } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import PropTypes from 'prop-types';
import { InputField } from '../../common/InputField';
import { SelectField } from '../../common/SelectField';
import { MultipleSelectField } from '../../common/MultipleSelectField';

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

function mapDispatchToProps(dispatch) {
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state) {
    const { settingData, updatePopupData, updateResponseData } = state;
    return Object.assign({}, { settingData, updatePopupData, updateResponseData });
}

export default connect(mapStateToProps, mapDispatchToProps)(UpdatePopup);