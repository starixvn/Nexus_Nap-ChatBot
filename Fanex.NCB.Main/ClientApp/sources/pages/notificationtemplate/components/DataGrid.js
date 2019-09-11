import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import Labels from '../../common/Labels';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';
import TemplatePreview from '../../common/TemplatePreview/index';

import Panel, { PanelHeader } from '../../common/panel/index';

export class DataGrid extends Component {
    constructor(props) {
        super(props);

        this.state = {
            expandPanels: false
        };

        this.panels = [];
    }

    toggleExpandPanels = () => {
        this.setState(prevState => ({ expandPanels: !prevState.expandPanels }), () => {
            if (this.props.gridData.total > 0) {
                this.panels.filter(x => x.state.isOpen !== this.state.expandPanels).forEach(panel => {
                    panel.set(this.state.expandPanels);
                });
            }
        });
    }

    onPaging = (pageIndex, pageSize) => {
        const params = { ...this.props.gridParams };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.setGridParams(params);
        this.props.getGridData(params);
    }

    buildHeader = () => {
        return (
            <Row >
                <Cell colWidth={5} header>{Labels.NumberCol}</Cell>
                <Cell colWidth={20} header>{Labels.NotificationType}</Cell>
                <Cell colWidth={20} header>{Labels.TemplateName}</Cell>
                <Cell colWidth={100} header>
                    <div className="has-icons">
                        {Labels.TitleContent}
                        <span className={this.state.expandPanels ? 'icon-collapse' : 'icon-expand'} onClick={this.toggleExpandPanels} />
                    </div>
                </Cell>
                <Cell colWidth={10} header>{Labels.Action}</Cell>
            </Row>);
    };

    buildBody = () => {
        const { pageIndex, pageSize } = this.props.gridParams;

        this.startIndex = (pageIndex - 1) * pageSize + 1;

        if (this.props.gridData.data.length > 0) {
            return this.props.gridData.data.map(this.renderRow);
        }

        return [<Row key={`body${1}`} ><Cell colSpan="5"><NoDataTitle /></Cell></Row>];
    }

    renderRow = (item, index) => (
        <Row key={item.notificationTemplateId} >
            <Cell className="text-center">{index + this.startIndex}</Cell>
            <Cell>{item.notificationTypeName}</Cell>
            <Cell>{item.notificationTemplateName}</Cell>
            <Cell>
                <Panel ref={panel => this.panels[index] = panel}>
                    <PanelHeader>
                        {item.title}
                    </PanelHeader>
                    <TemplatePreview text={item.content} />
                </Panel></Cell>
            <Cell className="text-center">
                <span className="icon-edit" data-notificationid={item.notificationTemplateId} onClick={this.handleCancelNotification} />
                <span className="icon-delete" data-notificationid={item.notificationTemplateId} onClick={this.handleCancelNotification} />
            </Cell>
        </Row>
    )

    render() {
        if (!this.props.gridData || !this.props.gridData.data) {
            return null;
        }

        const { total } = this.props.gridData;
        const { pageIndex, pageSize } = this.props.gridParams;

        const pagingOption = {
            PageIndex: pageIndex,
            PageSize: pageSize,
            TotalItem: total,
            PageList: [5, 10, 20, 50, 100, 500]
        };

        return (
            <Table
                width={1620}
                autoWidth={false}
                header={this.buildHeader()}
                body={this.buildBody()}
                isPaging
                pageOption={pagingOption}
                onPaging={this.onPaging}
                containerPadding={0}
                adjustedHeight={50}
            />
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { gridParams, gridData } = state;
    return {
        gridParams,
        gridData
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);