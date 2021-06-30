import React from 'react';
import { ToastContainer } from 'react-toastify';
import { shallow, mount } from 'enzyme';
import Layout from './../layout/Layout';
import { renderLayout } from './../layout/index';

import Sidebar from './../sidebar/index';
import Content from './../content/index';
import AuthRecheck from './../recheckin/index';

describe('Layout', () => {
    describe('render', () => {
        it('should match snapshot', () => {
            // Act
            const component = shallow(<Layout>{"Test"}</Layout>);

            // Assert
            expect(component.html()).toMatchSnapshot();
        });

        it('match snapshots with child component', () => {
            let children = <div>Test</div>;
            const component = shallow(
                <Layout>
                    {children}
                </Layout>
            );
            expect(component.contains(children)).toBeTruthy();
        });

        it('render correct child components', () => {
            let children = <div>Test</div>;
            const component = shallow(
                <Layout>
                    {children}
                </Layout>
            );
            expect(component.find(Sidebar)).toHaveLength(1);
            expect(component.find(Content)).toHaveLength(1);
            expect(component.find(ToastContainer)).toHaveLength(1);
            expect(component.find(AuthRecheck)).toHaveLength(1);
        });

        it('default isSidebarExpanded is true', () => {
            let children = <div>Test</div>;
            const wrapper = mount(
                <Layout>
                    {children}
                </Layout>
            );

            expect(wrapper.instance().state.isSidebarExpanded).toBeTruthy();

            wrapper.instance().collapseExpandHandler();
            wrapper.instance().forceUpdate();
            expect(wrapper.instance().state.isSidebarExpanded).not.toBeTruthy();
        });
    });

    describe('behavious', () => {
        it('Sidebar onchange call onMenuChange', () => {
            // Act
            const wrapper = shallow(<Layout>{"Test"}</Layout>);
            const spyOnMenuChange = jest.spyOn(wrapper.instance(), 'onMenuChange');
            let event = { target: { value: 'spy' } };
            wrapper.instance().forceUpdate();
            wrapper.find(Sidebar).simulate('change', event);

            // Assert
            expect(spyOnMenuChange).toHaveBeenCalled();
        });
    });
});

describe('RenderLayout function tests', () => {
    let container;

    beforeEach(() => {
        container = document.createElement('div');
        container.value = '';
        spyOn(document, 'getElementById').and.callFake(function () {
            return container;
        });
        document.body.appendChild(container);
    });

    afterEach(() => {
        document.body.removeChild(container);
        container = null;
    });

    it('Render content', () => {
        const content = (
            <div>Test Content</div>
        );
        const expected = shallow(<div><Layout>{content}</Layout></div>).html();

        renderLayout(content);

        expect(container.outerHTML).toEqual(expected.replace("/>", ">"));
    });
});