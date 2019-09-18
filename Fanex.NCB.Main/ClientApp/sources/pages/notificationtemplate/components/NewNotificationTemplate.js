import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { Button, Modal, ModalHeader, ModalBody, Form, FormGroup, Label, Input, FormFeedback, Col } from 'reactstrap';
import { Formik } from 'formik';
import * as Yup from 'yup';
import ReactMde, { commands } from "react-mde";
import Labels from '../../common/Labels';
import ResponseMessage from '../../common/response-message/ResponseMessage';
import TemplatePreview from '../../common/TemplatePreview/index';

const commandList = [
    {
        commands: [
            commands.boldCommand,
            commands.italicCommand,
            commands.strikeThroughCommand,
            commands.linkCommand
        ]
    }
];

class NewNotificationTemplate extends Component {
    constructor(props) {
        super(props);
    }

    toggleHandler = () => {
        this.props.toggleHandler();
    }

    renderNotificationTypeOption = item => <option key={item.notificationTypeID} value={item.notificationTypeID} >{item.notificationTypeName}</option>

    schema = Yup.object().shape({
        notificationTypeId: Yup.string().required(Labels.FieldRequired),
        notificationTemplateName: Yup.string().required(Labels.FieldRequired).max(100),
        title: Yup.string().required(Labels.FieldRequired).max(300),
        content: Yup.string().required(Labels.FieldRequired)
    })

    handleGerneratePreview = markdown => {
        return Promise.resolve(
            <TemplatePreview text={markdown} />
        );
    }

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
                <FormGroup row>
                    <Label for="notificationTypeId" className="form-label required" sm={2}>Notification Type</Label>
                    <Col sm={10}>
                        <Input type="select"
                            invalid={touched.notificationTypeId && errors.notificationTypeId}
                            name="notificationTypeId"
                            onChange={handleChange}
                            value={values.notificationTypeId}
                        >
                            <option value="" disabled hidden>{Labels.PleaseSelect}</option>
                            {this.props.notificationTypes.map(this.renderNotificationTypeOption)}
                        </Input>
                        {touched.notificationTypeId && errors.notificationTypeId && <FormFeedback>{errors.notificationTypeId}</FormFeedback>}
                    </Col>                   
                </FormGroup>
                <FormGroup row>                    
                    <Label for="title" className="form-label required" sm={2}>Template Name</Label>
                    <Col sm={10}>
                        <Input value={values.notificationTemplateName} name="notificationTemplateName"
                            invalid={touched.notificationTemplateName && errors.notificationTemplateName}
                            onChange={handleChange}
                        />
                        {touched.notificationTemplateName && errors.notificationTemplateName && <FormFeedback>{errors.notificationTemplateName}</FormFeedback>}
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="title" className="form-label required" sm={2}>Title</Label>
                    <Col sm={10}>
                        <Input value={values.title} name="title"
                            invalid={touched.title && errors.title}
                            onChange={handleChange}
                        />
                        {touched.title && errors.title && <FormFeedback>{errors.title}</FormFeedback>}
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="content" className="form-label required" sm={2}>Content</Label>

                    <Col sm={10}>
                        <ReactMde className={touched.content && errors.content && 'is-invalid'}
                            value={values.content} commands={commandList}
                            onChange={content => setFieldValue('content', content)}
                            selectedTab={values.mode}
                            onTabChange={selectedTab => setFieldValue('mode', selectedTab)}
                            generateMarkdownPreview={this.handleGerneratePreview}
                            maxEditorHeight={400}
                        />
                        {touched.content && errors.content && <FormFeedback>{errors.content}</FormFeedback>}
                    </Col>
                </FormGroup>

                <Button className="btn-submit float-right" disabled={isSubmitting} type="submit">{Labels.Submit}</Button>
            </Form >
        );
    }

    handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        this.props.createTemplate(values).then(() => {
            if (this.props.createTemplateResult.isSuccessful) {
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
            <Modal isOpen toggle={this.props.onClose} className="new-notification-template" backdrop="static">
                <ModalHeader toggle={this.props.onClose}>Create New Notification Template</ModalHeader>
                <ModalBody>
                    {this.props.createTemplateResult &&
                        <ResponseMessage isSuccessful={this.props.createTemplateResult.isSuccessful}
                            message={this.props.createTemplateResult.message || Labels.SubmitSuccessfully}
                        />
                    }
                    <Formik ref="form"
                        initialValues={{
                            notificationTypeId: '',
                            notificationTemplateName: '',
                            title: '',
                            content: '',
                            mode: 'write'
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
        createTemplate: ActionCreators.createTemplate
    }, dispatch);
};

const mapStateToProps = state => {
    const { createTemplateResult, notificationTypes } = state;
    return {
        createTemplateResult,
        notificationTypes
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(NewNotificationTemplate);