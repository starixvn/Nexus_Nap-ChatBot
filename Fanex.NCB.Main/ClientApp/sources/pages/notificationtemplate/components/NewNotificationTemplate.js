import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Button, Modal, ModalHeader, ModalBody, Form, FormGroup, Label, Input, FormFeedback, Col } from 'reactstrap';

import { Formik } from 'formik';
import * as Yup from 'yup';
import ReactMde, { commands } from "react-mde";
import ReactMarkdown from 'react-markdown/with-html';
import "react-mde/lib/styles/css/react-mde-all.css";
import Labels from '../../common/Labels';

const commandList = [
    {
        commands: [
            commands.headerCommand,
            commands.boldCommand,
            commands.italicCommand,
            commands.strikeThroughCommand,
            commands.linkCommand,
            commands.quoteCommand
        ]
    }
];

Yup.addMethod(Yup.string, 'notmatch', function (regex) {
    regex.exec
});

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
        let params = markdown.match(/{[-\w\s]+}/g);

        if (params) {
            for (let i = 0; i < params.length; i++) {
                markdown = markdown.replace(params[i], `<span style="color:red">${params[i]}</span>`);
            }
        }

        return Promise.resolve(
            <ReactMarkdown source={markdown} escapeHtml={false} />
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
                    <Col sm={4}>
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
                    <Label for="title" className="form-label required" sm={2}>Template Name</Label>
                    <Col sm={4}>
                        <Input value={values.title} name="title"
                            invalid={touched.title && errors.title}
                            onChange={handleChange}
                        />
                        {touched.title && errors.title && <FormFeedback>{errors.title}</FormFeedback>}
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="title" className="form-label required" sm={2}>Title</Label>
                    <Col sm={10}>
                        <Input value={values.notificationTemplateName} name="notificationTemplateName"
                            invalid={touched.notificationTemplateName && errors.notificationTemplateName}
                            onChange={handleChange}
                        />
                        {touched.notificationTemplateName && errors.notificationTemplateName && <FormFeedback>{errors.notificationTemplateName}</FormFeedback>}
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
    render() {
        return (
            <div>
                <Modal isOpen toggle={this.props.onClose} className="new-notification-template" backdrop="static" >
                    <ModalHeader toggle={this.props.onClose}>Create New Notification Template</ModalHeader>
                    <ModalBody>

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
            </div>
        );
    }
}

export default connect()(NewNotificationTemplate);