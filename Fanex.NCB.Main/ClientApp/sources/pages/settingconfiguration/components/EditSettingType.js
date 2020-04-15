import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { Button, Modal, ModalHeader, ModalBody, Form, FormGroup, Label, Input, FormFeedback, Col } from 'reactstrap';
import { Formik } from 'formik';
import { object, string } from 'yup';
import Labels from '../../common/Labels';
import ResponseMessage from '../../common/response-message/ResponseMessage';
import TemplatePreview from '../../common/TemplatePreview/index';

class EditNotificationType extends Component {
    constructor(props) {
        super(props);

        this.state = {
            settingType: null
        };
    }

    toggleHandler = () => {
        this.props.toggleHandler();
    }

    schema = object().shape({
        type: string().required(Labels.FieldRequired),
        name: string().required(Labels.FieldRequired).max(250)
    })

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting
    }) => {
        return (
            <Form onSubmit={handleSubmit}>
                <FormGroup>
                    <Label for="type" className="form-label required">Msg Contents Type</Label>
                    <div>
                        <Input value={values.type} name="type"
                            invalid={touched.type && errors.type}
                            onChange={handleChange}
                            disabled
                        />
                        {touched.type && errors.type && <FormFeedback>{errors.type}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="name" className="form-label required">Name</Label>
                    <div>
                        <Input value={values.name} name="name"
                            invalid={touched.name && errors.name}
                            onChange={handleChange}
                        />
                        {touched.name && errors.name && <FormFeedback>{errors.name}</FormFeedback>}
                    </div>
                </FormGroup>
                <div className="list-button">
                    <Button className="btn-submit float-right" disabled={isSubmitting} type="submit">{Labels.Save}</Button>
                </div>
            </Form >
        );
    }

    handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        this.props.editSettingType(values).then(() => {
            if (this.props.editSettingTypeResult.isSuccessful) {
                setTimeout(() => {
                    this.props.onEditSuccess();
                }, 1000);
            } else {
                setSubmitting(false);
            }
        });
    }

    handleClose = () => {
        this.props.setEditSettingTypeResult(null);
        this.props.setEditSettingTypePopup({ show: false });
    }

    render() {
        return (
            <Modal isOpen toggle={this.handleClose} className="new-setting-type" backdrop="static">
                <ModalHeader toggle={this.handleClose}>Edit Msg Contents Type</ModalHeader>
                <ModalBody>
                    {this.props.editSettingTypeResult &&
                        <ResponseMessage isSuccessful={this.props.editSettingTypeResult.isSuccessful}
                            message={this.props.editSettingTypeResult.message || Labels.EditMsgContentsTypeSuccessfully}
                        />
                    }
                    <Formik ref="form"
                        initialValues={this.props.editSettingTypePopup.settingType}
                        render={this.renderForm}
                        validationSchema={this.schema}
                        onSubmit={this.handleSubmit}
                    />
                </ModalBody>
            </Modal>
        );
    }
}

const mapDispatchToProps = dispatch => {
    return bindActionCreators({
        editSettingType: ActionCreators.editSettingType,
        setEditSettingTypePopup: ActionCreators.setEditSettingTypePopup,
        setEditSettingTypeResult: ActionCreators.setEditSettingTypeResult
    }, dispatch);
};

const mapStateToProps = state => {
    const { editSettingTypeResult, editSettingTypePopup, notificationTypes } = state;
    return {
        editSettingTypeResult,
        editSettingTypePopup,
        notificationTypes
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(EditNotificationType);