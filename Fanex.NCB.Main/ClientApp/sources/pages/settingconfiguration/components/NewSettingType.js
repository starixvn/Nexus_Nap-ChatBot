import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { Button, Modal, ModalHeader, ModalBody, Form, FormGroup, Label, Input, FormFeedback, Col } from 'reactstrap';
import { Formik } from 'formik';
import { object, string } from 'yup';
import Labels from '../../common/Labels';
import ResponseMessage from '../../common/response-message/ResponseMessage';
class NewSettingType extends Component {
    constructor(props) {
        super(props);
    }

    toggleHandler = () => {
        this.props.toggleHandler();
    }

    schema = object().shape({
        type: string().required(Labels.FieldRequired).max(10),
        name: string().required(Labels.FieldRequired).max(60)
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
                        <Input invalid={touched.type && errors.type}
                            name="type"
                            onChange={handleChange}
                            value={values.type}
                            maxLength={10}
                        />
                        {touched.type && errors.type && <FormFeedback>{errors.type}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="name" className="form-label required">Name</Label>
                    <div>
                        <Input invalid={touched.name && errors.name}
                            name="name"
                            onChange={handleChange}
                            value={values.name}
                            maxLength={60}
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
        this.props.createSettingType(values).then(() => {
            if (this.props.createSettingTypeResult.isSuccessful) {
                setTimeout(() => {
                    this.props.onCreateSuccess();
                }, 1000);
            } else {
                setSubmitting(false);
            }
        });
    }

    render() {
        return (
            <Modal isOpen toggle={this.props.onClose} className="new-setting-type" backdrop="static">
                <ModalHeader toggle={this.props.onClose}>New Msg Contents Type</ModalHeader>
                <ModalBody>
                    {this.props.createSettingTypeResult &&
                        <ResponseMessage isSuccessful={this.props.createSettingTypeResult.isSuccessful}
                            message={this.props.createSettingTypeResult.message || Labels.CreateNewMsgContentsTypeSuccessfully}
                        />
                    }
                    <Formik ref="form"
                        initialValues={{
                            type: '',
                            name: ''
                        }}
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
        createSettingType: ActionCreators.createSettingType
    }, dispatch);
};

const mapStateToProps = state => {
    const { createSettingTypeResult } = state;
    return {
        createSettingTypeResult
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(NewSettingType);