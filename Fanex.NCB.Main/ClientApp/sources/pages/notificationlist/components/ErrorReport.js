import React, { Component } from 'react';
import { Modal, ModalHeader, ModalBody } from 'reactstrap';
import { DateFormat, DateTimeFormat, NotificationStatus } from '../../constants/constants';
import ResponseMessage from '../../common/response-message/ResponseMessage';

import './ErrorReport.scss';

export default class ErrorReport extends Component {
    constructor(props) {
        super(props);

        this.state = {
            remark: '',
            remarkTouched: false
        };
    }

    handleRemarkChange = event => this.setState({ remark: event.target.value.trim(), remarkTouched: true });

    renderFailedRecipient = (recipient, index) => (
        <tr key={index}>
            <td>{recipient.recipientName}</td>
            <td>{recipient.recipientTypeName}</td>
            <td>{recipient.groupName}</td>
            <td className="text-red">{recipient.errorMessage}</td>
        </tr>
    )

    copyToClipboard = () => {
        let input = document.createElement("input");
        input.value = this.props.report.successRecipients.join(', ');
        document.body.appendChild(input);
        input.select();
        document.execCommand('copy');
        document.body.removeChild(input);
    }

    handleResolveNotification = () => {
        if (this.state.remark !== '') {
            this.props.onResolveNotification(this.state.remark);
        } else {
            if (!this.state.remarkTouched) {
                this.setState({ remarkTouched: true });
            }
        }
    }

    render() {
        const { isOpen, report, toggle, responseMessage } = this.props;

        if (report) {
            return (
                <Modal isOpen={isOpen} toggle={toggle} className="error-report" backdrop="static">
                    <ModalHeader toggle={toggle}>Notification Sending Error Report</ModalHeader>
                    <ModalBody>
                        {responseMessage &&
                            <ResponseMessage isSuccessful={responseMessage.isSuccessful}
                                message={responseMessage.message}
                            />
                        }
                        <p>
                            Error report for the notification titled:<br />
                            <span className="bold text-uppercase">{report.title} - {report.configSendTime.toFormat(DateFormat)}</span>
                        </p>
                        <p>
                            Send at:<br />
                            <span className="bold">{report.configSendTime.toFormat(DateTimeFormat)}</span>
                        </p>
                        <div className="msg-success">Notification has been successfully sent to the following recipient(s): <strong>{report.successRecipients.join(', ')}</strong></div>
                        <div className="msg-error">Error(s) occoured while sending the notification to the following recipient(s)</div>
                        <table className="tbl w-100">
                            <thead>
                                <tr>
                                    <th>
                                        <div className="has-icons">Recipient<span className="icon-copy" onClick={this.copyToClipboard} /></div>
                                    </th>
                                    <th>Recipient Type</th>
                                    <th>Group</th>
                                    <th>Reason</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr >
                                    <td>recipient.recipientName</td>
                                    <td>recipient.recipientTypeName</td>
                                    <td>recipient.groupName</td>
                                    <td className="text-red">recipient.errorMessage</td>
                                </tr>
                                {report.errorDetails.map(this.renderFailedRecipient)}
                            </tbody>
                        </table>
                        <div className="remark">
                            <span>Remark:</span>
                            <input type="text" className={`form-control ${this.state.remarkTouched && this.state.remark === '' && 'is-invalid'}`}
                                onChange={this.handleRemarkChange}
                                value={this.state.remark}
                            />
                            <button className="btn-mark" onClick={this.handleResolveNotification}>Mark As Resolved</button>
                        </div>
                        <div className="resolved-remark">
                            <p>Remark: <span className="text-italic">Remark Text{report.resolvedRemark}</span></p>
                            <p><span className="text-red">*</span> Resolved by: <strong>{report.resolvedByUsername}</strong></p>
                        </div>
                    </ModalBody>
                </Modal>
            );
        }

        return null;
    }
}