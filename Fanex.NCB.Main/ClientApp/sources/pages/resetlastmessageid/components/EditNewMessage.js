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
import { element } from 'prop-types';
var dateFormat = require("dateformat");

export class EditNewMessage extends Component {
    constructor(props) {
        super(props);

        this.state = {
            expandPanels: false
        };

        this.panels = [];
        this.selectedMessageId = this.props.newMaxId;
    }

    handleClose = () => {
        this.props.setEditNewMessagePopup({ show: false });
    };

    toggleExpandPanels = () => {
        const missingData = this.props.data;
        this.setState(prevState => ({ expandPanels: !prevState.expandPanels }), () => {
            if (missingData.length > 0) {
                this.panels.filter(x => x && (x.state.isOpen !== this.state.expandPanels)).forEach(panel => {
                    panel.handleSet(this.state.expandPanels);
                });
            }
        });
    };

    isRenderMsgTargetId = (id, settingName) => {
        if (id === 6 || id === 7 || (id === 2 && settingName === 'LastMsgTargetId')) {
            return true;
        }
        return false;
    };

    buildHeader = () => {
        const { id, settingName } = this.props;
        return (
            <Row>
                <Cell colWidth={7} header>{Labels.Select}</Cell>
                <Cell colWidth={12} header>{(id === 4 || id === 5) ? Labels.ID : Labels.MessageID}</Cell>
                {
                    id === 2 && settingName === 'LastVoidingHistoryCollectionId' &&
                    <Cell colWidth={12} header>{Labels.VoidingHistoryCollectionID}</Cell>
                }
                {
                    this.isRenderMsgTargetId(id, settingName) &&
                    <Cell colWidth={12} header>{Labels.MsgTargetID}</Cell>
                }
                <Cell colWidth={43} header>
                    <div className="has-icons">
                        {Labels.Content_en}
                        <span className={this.state.expandPanels ? 'icon-collapse' : 'icon-expand'} onClick={this.toggleExpandPanels} />
                    </div>
                </Cell>
                <Cell colWidth={(id === 6 || id === 7) ? 17 : 14} header>{`${(id === 4 || id === 5) ? Labels.Date : Labels.PostTime} (GMT +8)`}</Cell>
            </Row>
        );
    };

    buildBody = () => {
        const missingData = this.props.data;

        if (missingData.length > 0) {
            return missingData.map((item, index) => this.renderRow(item, index));
        }

        return [<Row key={`body${1}`} ><Cell colSpan="4"><NoDataTitle /></Cell></Row>];
    };

    handleChangeCheck = event => {
        this.selectedMessageId = event.target.value;
    };

    handleSubmit = () => {
        const messageIds = this.props.selected.map(item => {
            const element = { ...item };
            if (item.id === this.props.id && item.settingName === this.props.settingName) {
                element.newMaxMessageId = parseInt(this.selectedMessageId, 10);
            }
            return element;
        });
        console.log('messageIds', messageIds);
        this.props.setSelected(messageIds);
        this.handleClose();
    };

    getValueByJobIdentify = item => {
        const msg = item.settingName === 'LastVoidingHistoryCollectionId' ? 'voidingHistoryCollectionId' : 'msgTargetId';
        const identifyMapping = {
            2: msg,
            6: 'msgTargetId',
            7: 'msgTargetId'
        };

        return item[identifyMapping[item.jobIdentify]] || item.messageId;
    };

    renderRow = (item, index) => {
        const { settingName } = this.props;
        const msgId = (item.jobIdentify === 2 && settingName === 'LastVoidingHistoryCollectionId') ? item.messageIds?.split(',').join(', ') : item.messageId;

        console.log('item', item);
        return (
            <Row key={index} >
                <Cell className="text-center">
                    <Input type="radio" name="radioEditNewMessage" value={this.getValueByJobIdentify(item)}
                        defaultChecked={this.getValueByJobIdentify(item) === this.props.newMaxId}
                        onChange={event => this.handleChangeCheck(event)}
                    />
                </Cell>
                <Cell className="text-center">{msgId}</Cell>
                {
                    item.jobIdentify === 2 && settingName === 'LastVoidingHistoryCollectionId' &&
                    <Cell className="text-center">{item.voidingHistoryCollectionId}</Cell>
                }
                {
                    this.isRenderMsgTargetId(item.jobIdentify, settingName) &&
                    <Cell className="text-center">{item.msgTargetId}</Cell>
                }
                <Cell>
                    {
                        (item.content && item.content.length > 75) ? <PanelContent ref={panel => this.panels[index] = panel} content={item.content} />
                            : <ContentPreview text={item.content} />
                    }
                </Cell>
                <Cell className="text-center">{dateFormat(item.postTime, DateTimeFormatV3)}</Cell>
            </Row>
        )
    };

    render() {
        const { data, id } = this.props;
        if (data === null)
            return null;

        return (
            <Modal isOpen toggle={this.handleClose} className="edit-new-message" backdrop="static" size="lg">
                <ModalHeader toggle={this.handleClose}>
                    {`${Labels.ResetLastIdFor} ${this.props.title}`}
                </ModalHeader>
                <ModalBody>
                    <Label className="form-label">{(id === 4 || id === 5) ? Labels.ListOfMissingID : Labels.ListOfMissingMessages}</Label>
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
    const { selected } = state;
    return { selected };
};

export default connect(mapStateToProps, mapDispatchToProps)(EditNewMessage);