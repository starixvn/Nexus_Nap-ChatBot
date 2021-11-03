import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { Modal, ModalHeader, ModalBody, Input, Label } from 'reactstrap';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import { DateTimeFormatV3 } from '../../constants/constants';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';
import Labels from '../../common/Labels';
import PanelContent from '../../common/panel-content/PanelContent';
import ContentPreview from '../../common/ContentPreview';
var dateFormat = require("dateformat");

export class EditNewMessage extends Component {

    constructor(props) {
        super(props);

        this.state = {
            expandPanels: false
        };

        this.panels = [];
        this.selectedMessageId = 0;
    }

    handleClose = () => {
        this.props.setEditNewMessagePopup({ show: false });
    };

    toggleExpandPanels = () => {
        const data = this.props.missingMessages.filter(item => item.jobIdentify === this.props.id);
        this.setState(prevState => ({ expandPanels: !prevState.expandPanels }), () => {
            if (data.length > 0) {
                this.panels.filter(x => x && (x.state.isOpen !== this.state.expandPanels)).forEach(panel => {
                    panel.handleSet(this.state.expandPanels);
                });
            }
        });
    };

    buildHeader = () => {
        return (
            <Row>
                <Cell colWidth={5} header>{Labels.Select}</Cell>
                <Cell colWidth={8} header>{Labels.MessageID}</Cell>
                <Cell colWidth={45} header>
                    <div className="has-icons">
                        {Labels.Content_en}
                        <span className={this.state.expandPanels ? 'icon-collapse' : 'icon-expand'} onClick={this.toggleExpandPanels} />
                    </div>
                </Cell>
                <Cell colWidth={14} header>{`${Labels.PostTime} (GMT +8)`}</Cell>
            </Row>
        );
    };

    buildBody = () => {
        const data = this.props.missingMessages.filter(item => item.jobIdentify === this.props.id);

        if (data.length > 0) {
            return data.map((item, index) => this.renderRow(item, index));
        }

        return [<Row key={`body${1}`} ><Cell colSpan="4"><NoDataTitle /></Cell></Row>];
    };

    handleChangeCheck = event => {
        this.selectedMessageId = event.target.value;
    };

    handleSubmit = () => {
        const messageId = this.props.selected.map(item => {
            const element = {...item};
            if (item.id === this.props.id) {
                element.newMaxMessageId = parseInt(this.selectedMessageId, 10);
            }
            return element;
        });
        this.props.setSelected(messageId);
        this.handleClose();
    };

    renderRow = (item, index) => (
        <Row key={index} >
            <Cell className="text-center">
                <Input type="radio" name="radioEditNewMessage" value={item.messageId}
                    defaultChecked = { item.messageId === this.props.newMaxId }
                    onChange={event => this.handleChangeCheck(event)}
                />
            </Cell>
            <Cell className="text-center">{item.messageId}</Cell>
            <Cell>
                {
                    (item.content && item.content.length > 75) ? <PanelContent ref={panel => this.panels[index] = panel} content={item.content}/>
                    : <ContentPreview text={item.content} />
                }
            </Cell>
            <Cell className="text-center">{dateFormat(item.postTime, DateTimeFormatV3)}</Cell>
        </Row>
    );

    render() {
        const { missingMessages } = this.props;
        if (missingMessages === null)
            return null;

        return (
            <Modal isOpen toggle={this.handleClose} className="edit-new-message" backdrop="static" size="lg">
                <ModalHeader toggle={this.handleClose}>
                    {`${Labels.NewPreviousMaxMessageIDFor} ${this.props.title}`}
                </ModalHeader>
                <ModalBody>
                    <Label className="form-label">List of missing messages:</Label>
                    <Table
                        width={925}
                        bodyHeight={215}
                        autoWidth={false}
                        autoHeight={false}
                        header={this.buildHeader()}
                        body={this.buildBody()}
                        containerPadding={0}
                        adjustedHeight={50}
                    />
                    <div className="pt-3">
                        <button className="btn-cancel float-right" onClick={() => this.handleClose()}>{Labels.Cancel}</button>
                        <button className="btn-submit float-right mr-3" onClick={() => this.handleSubmit()}>{Labels.Ok}</button>
                    </div>
                </ModalBody>
            </Modal>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { missingMessages, selected } = state;
    return { missingMessages, selected };
};

export default connect(mapStateToProps, mapDispatchToProps)(EditNewMessage);