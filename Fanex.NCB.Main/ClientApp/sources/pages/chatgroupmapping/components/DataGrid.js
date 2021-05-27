import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import axios from 'axios';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import { ActionCreators } from '../actions';
import CommonFunctions from '../../common/common-functions/CommonFunctions';
import { PAGING_SIZES, mappingStatus } from '../../constants/constants';
import Labels from '../../common/Labels';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';
import Site from '../../lib/site';
import ConfirmPopup from '../../common/confirm-popup/ConfirmPopup';

export class DataGrid extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isShowActivePopup: false,
            item: {},
            activeContent: ''
        }
    }

    onPaging = (pageIndex, pageSize) => {
        const params = { ...this.props.gridParams };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.setGridParams(params);
        this.props.getGridData(params);
    }

    openEditPopup = item => {
        if (!CommonFunctions.isBotRemoved(item.mappingStatusID)) {
            this.props.filterRecipients({ recipients: this.props.settings.recipients, recipientTypeId: item.recipientTypeID });
            this.props.setEditPopupData({
                isShowed: true,
                editingItem: {
                    groupID: item.groupID,
                    conversationID: item.conversationID,
                    mappingStatusId: item.mappingStatusID,
                    mappingStatusName: item.mappingStatusName,
                    botName: item.botName,
                    groupName: item.groupName,
                    recipientTypeId: {
                        value: item.recipientTypeID,
                        isValid: item.recipientTypeID !== 0
                    },
                    recipientIds: {
                        value: item.recipientIDs,
                        isValid: item.recipientIDs !== ''
                    },
                    recipientStatusNames: item.recipientStatusNames,
                    chatGroupTypeId: {
                        value: item.chatGroupTypeID,
                        isValid: item.chatGroupTypeID !== 0
                    },
                    remark: {
                        value: item.remark,
                        isValid: item.remark !== null
                    },
                    channelId: item.channelId,
                    channelName: item.channelName
                }
            });
        }
    }

    buildHeader = (isHost) => {
        return (
            <Row style={{ height: 32 }}>
                <Cell colWidth={50} header>{Labels.NumberCol}</Cell>
                <Cell colWidth={100} header>{Labels.BotName}</Cell>
                <Cell colWidth={80} header>{Labels.BotService}</Cell>
                <Cell colWidth={60} header>{Labels.GroupID}</Cell>
                <Cell colWidth={200} header>{Labels.GroupName}</Cell>
                <Cell colWidth={110} header>{Labels.ChatGroupType}</Cell>
                <Cell colWidth={100} header>{Labels.RecipientType}</Cell>
                <Cell colWidth={130} header>{Labels.Recipient}</Cell>
                <Cell colWidth={100} header>{Labels.LicenseeStatus}</Cell>
                <Cell colWidth={100} header>{Labels.MappingStatus}</Cell>
                <Cell colWidth={50} header>{Labels.Active}</Cell>
                <Cell colWidth={200} header>{Labels.Remark}</Cell>
                {
                    isHost &&
                    <Cell colWidth={60} header>{Labels.Action}</Cell>
                }
            </Row>);
    };

    copyGroupIdToClipboard = text => {
        const input = document.createElement('textarea');
        input.value = text;
        document.body.appendChild(input);
        input.select();
        document.execCommand('copy');
        input.remove();
        CommonFunctions.displayCopiedToClipboard();
    }

    getLicenseeStatus = item => item.recipientTypeName ? CommonFunctions.getLicenseeStatus(item.recipientStatusNames) : '';

    getEditClassNames = item => CommonFunctions.isBotRemoved(item.mappingStatusID) ? 'icon-edit edit-disabled' : 'icon-edit';

    updateActive = async () => {
        const params = {
            groupID: this.state.item.groupID,
            isActive: !this.state.item.isActive
        };
        this.props.editActive(params).then(response => {
            const data = response.data;
            if (data && data.isSuccessful) {
                this.props.getGridData(this.props.gridParams);
            }

            this.toggleActive();
        });
    };

    changeActive = item => {
        this.setState({
            isShowActivePopup: true,
            item: item,
            activeContent: item.isActive ? Labels.ConfirmUpdateActive : Labels.ConfirmUpdateInActive
        })
    }

    toggleActive = () => {
        this.setState({ isShowActivePopup: !this.state.isShowActivePopup });
    }

    buildBody = (data, pageIndex, pageSize, isHost) => {
        const rows = [];
        const startIndex = (pageIndex - 1) * pageSize + 1;
        if (data.length > 0) {
            data.map((item, index) => {
                rows.push(<Row key={`body${index}`} >
                    <Cell className="text-center">{index + startIndex}</Cell>
                    <Cell>{item.mappingStatusID === mappingStatus.BotRemoved ? '' : item.botName}</Cell>
                    <Cell>{item.channelName}</Cell>
                    <Cell className="text-center">
                        <span
                            onClick={() => this.copyGroupIdToClipboard(item.conversationID)}
                            className="icon-copy" title={item.conversationID}
                        />
                    </Cell>
                    <Cell>{item.groupName}</Cell>
                    <Cell>{item.chatGroupTypeName}</Cell>
                    <Cell>{item.recipientTypeName}</Cell>
                    <Cell>{item.recipientNames}</Cell>
                    <Cell>{this.getLicenseeStatus(item)}</Cell>
                    <Cell>
                        <span className={CommonFunctions.getMappingClass(item.mappingStatusID)}>{item.mappingStatusName}</span>
                    </Cell>
                    <Cell className="text-center">
                        <input id={index + startIndex}
                            className="checkbox-custom"
                            type='checkbox'
                            checked={item.isActive}
                            disabled={!(isHost && (CommonFunctions.isBotRemoved(item.mappingStatusID) || CommonFunctions.isMapped(item.mappingStatusID)))}
                            onChange={() => this.changeActive(item)}
                        />
                        <label htmlFor={index + startIndex}/>
                    </Cell>
                    <Cell>{item.remark}</Cell>
                    {
                        isHost &&
                        <Cell className="text-center col-action">
                            <div>
                                {
                                    item.mappingStatusID !== mappingStatus.BotRemoved &&
                                    <span className={this.getEditClassNames(item)} id={'edit-' + index} onClick={() => this.openEditPopup(item)} />
                                }
                            </div>
                        </Cell>
                    }
                </Row>);
            });
        } else {
            rows.push(<Row key={`body${1}`} ><Cell colSpan={isHost ? 13 : 12}><NoDataTitle /></Cell></Row >);
        }

        return rows;
    }

    render() {
        if (!this.props.gridData || !this.props.gridData.data) {
            return null;
        }

        const { data, count, isHost } = this.props.gridData;
        const { pageIndex, pageSize } = this.props.gridParams;

        const pagingOption = {
            PageIndex: pageIndex,
            PageSize: pageSize,
            TotalItem: count,
            PageList: PAGING_SIZES
        };

        return (
            <div className="table-container-div">
                <Table
                    width={1620}
                    autoWidth={false}
                    header={this.buildHeader(isHost)}
                    body={this.buildBody(data, pageIndex, pageSize, isHost)}
                    isPaging
                    pageOption={pagingOption}
                    onPaging={this.onPaging}
                    containerPadding={0}
                    adjustedHeight={50}

                />
                <ConfirmPopup
                    isOpen={this.state.isShowActivePopup}
                    onConfirm={this.updateActive}
                    onToggle={this.toggleActive}
                    content={this.state.activeContent}
                />
            </div>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { gridParams, gridData, settings } = state;
    return {
        gridParams,
        gridData,
        settings
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);